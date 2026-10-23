/** Coordinates account state, navigation, documents, assessments and feedback. */
import useConsent from '../privacy/useConsent';
import type {FeedbackKind} from '../feedback/types';
import type { CvFeedback } from '../analysis/feedback';
import useOpportunityNotifications from '../notifications/useOpportunityNotifications';
import useCareerAssessment from '../dashboard/useCareerAssessment';
import useCampaignMedals from '../achievements/useCampaignMedals';
import useLearningProgress from '../learning/useLearningProgress';
import useOpportunities from '../opportunities/useOpportunities';
import useAnalysisRunner from '../upload/useAnalysisRunner';
import {documentEntry} from '../upload/documentFlow';
import {apiRequest, ApiError} from '../api/client';
import {cvAnalysisSource, type CVAnalysis} from '../api/cvs';
import {transcriptAnalysisSource, type TranscriptAnalysis} from '../api/transcripts';
import {loadProfile} from '../api/profile';
import { useCallback, useEffect, useMemo, useReducer, useRef, useState } from 'react';
import { Alert, Platform, ScrollView, useWindowDimensions } from 'react-native';
import * as DocumentPicker from '../platform/documentPicker';
import { defaultAccount, type Account } from '../auth/account';
import { type Screen } from './types';
import type { LegalDocumentType } from '../legal/types';
import useMiraChat from '../chat/useMiraChat';
import { analysisReducer, initialAnalysisState } from '../upload/analysisState';
import createNavigationHistory from './navigationHistory';
import {clearSession, loadSession, saveSession} from './sessionStorage';
export type CvAssessment = {documentUri:string; fileName?:string; role:number; score:number|null; atsScore?:number; formattingScore?:number; feedback?:CvFeedback; headline:string; summary:string; findings:string[]; analysedAt:string; strengths?:{title:string;description:string}[]; improvements?:{title:string;description:string;priority?:'high'|'medium'|'low'}[]; learningTopic?:string};
/** Coordinates account state, navigation, documents, assessments and feedback. */
export default function useAppModel() {
    const { width } = useWindowDimensions();
    const stored = useRef(loadSession()).current;
    const [screen, setScreen] = useState<Screen>(stored?.screen ?? 'splash');
    const currentScreen=useRef<Screen>(stored?.screen ?? 'splash');
    currentScreen.current=screen;
    const navigationHistory=useRef(createNavigationHistory()).current;
    const [legalDocument, setLegalDocument] = useState<LegalDocumentType | null>(null);
    /** Opens the requested legal document. */
    const openLegal =  (document: LegalDocumentType) => setLegalDocument(document);
    /** Closes the legal document view. */
    const closeLegal =  () => setLegalDocument(null);
    const [account, setAccount] = useState<Account>(stored ? {...defaultAccount, token: stored.token, email: stored.email, name: stored.name} : defaultAccount);
    useEffect(() => { saveSession(account, screen); }, [account, screen]);
    const mira = useMiraChat(account.token);
    const [error, setError] = useState('');
    useEffect(() => {
        if (!account.token) return;
        const controller = new AbortController();
        loadProfile(account.token, controller.signal).then(profile => {
            if (controller.signal.aborted) return;
            setAccount(current => current.token !== account.token ? current : ({
                ...current,
                name: `${profile.fName} ${profile.lName}`.trim(),
                firstName: profile.fName,
                lastName: profile.lName,
                qualification: profile.qualification,
                fieldOfStudy: profile.fieldOfStudy,
                experienceLevel: profile.experienceLevel ?? '',
                interests: profile.interests ?? '',
                industry: profile.industry ?? '',
                phone: profile.phone ?? '',
                targetRoles: profile.targetRoles ?? [],
                workAreas: profile.workAreas ?? [],
                location: (profile.workAreas ?? []).map(area => area.label).join('; '),
                targetCareerID: profile.targetCareerID ?? undefined,
            }));
        }).catch(error => {
            if (controller.signal.aborted) return;
            if (error instanceof ApiError && error.status === 401) {
                clearSession(); setAccount(defaultAccount); setScreen('signin'); currentScreen.current = 'signin';
                return;
            }
            if (!(error instanceof ApiError && error.status === 404))
                setError(error instanceof Error ? error.message : 'Could not load your profile.');
        });
        return () => controller.abort();
    }, [account.token]);
    const consent=useConsent(account.email);
    const [feedback,setFeedback]=useState<{kind:FeedbackKind;message?:string;next:Screen}|null>(null);
    /** Selects an outcome message and its continuation screen. */
    const showFeedback= (kind:FeedbackKind,next:Screen=screen,message?:string)=>setFeedback({kind,next,message});
    const roles = account.targetRoles || [];
    const opportunityModel = useOpportunities(account.location, account.email, account.workAreas, account.token);
    const opportunityNotifications = useOpportunityNotifications(account.email, opportunityModel.allOpportunities);
    const [cvAssessments,setCvAssessments] = useState<Record<string,CvAssessment|null>>({});
    const rawCvAssessment=cvAssessments[account.email.toLowerCase()];
    /** Stores CV assessment results for the current account. */
    const setCvAssessment= (value:CvAssessment|null)=>setCvAssessments(
        /** Builds the next cv assessments value from its previous state. */
         old=>({...old,[account.email.toLowerCase()]:value}));
    const [file, setFile] = useState('');
    const [cvDocument, setCvDocument] = useState<DocumentPicker.DocumentPickerAsset | null>(null);
    const [transcriptDocument, setTranscriptDocument] = useState<DocumentPicker.DocumentPickerAsset | null>(null);
    const [transcriptAssessment, setTranscriptAssessment] = useState<(TranscriptAnalysis & {documentUri: string; owner: string}) | null>(null);
    const transcriptSource = useMemo(() => account.token ? transcriptAnalysisSource(account.token, (result, documentUri) => setTranscriptAssessment({...result, documentUri, owner: account.email.toLowerCase()})) : undefined, [account.token, account.email]);
    /** Releases the previous CV preview when its selection changes. */
    useEffect(() => () => { if (cvDocument) DocumentPicker.releaseDocument(cvDocument.uri); }, [cvDocument]);
    /** Releases the previous transcript preview when its selection changes. */
    useEffect(() => () => { if (transcriptDocument) DocumentPicker.releaseDocument(transcriptDocument.uri); }, [transcriptDocument]);
    const [cvAnalysis, updateCvAnalysis] = useReducer(analysisReducer, undefined, initialAnalysisState);
    const [transcriptAnalysis, updateTranscriptAnalysis] = useReducer(analysisReducer, undefined, initialAnalysisState);
    useEffect(() => {
        if (!account.token || cvDocument) return;
        const controller = new AbortController();
        const owner = account.email.toLowerCase();
        apiRequest<CVAnalysis>('/api/cvs/current/analysis', {token: account.token, signal: controller.signal}).then(async result => {
            if (controller.signal.aborted || !result.analysisID) return;
            let fileName: string | undefined;
            try {
                const cv = await apiRequest<{cvid: string; fileName: string}>('/api/cvs/current', {token: account.token, signal: controller.signal});
                if (cv.cvid === result.cvid) fileName = cv.fileName;
            } catch { /* The report remains available if metadata cannot be loaded. */ }
            if (controller.signal.aborted) return;
            const documentUri = `saved:${result.analysisID}`;
            setCvAssessments(old => ({...old, [owner]: {documentUri, fileName, role: 0, score: result.qualityScore, atsScore: result.atsScore, formattingScore: result.formattingScore, headline: 'CV analysis', summary: result.recommendations, findings: [result.weaknesses].filter(Boolean), analysedAt: result.analysedAt, feedback: {sections: [{heading: 'Strengths', body: result.strengths}, {heading: 'Areas to improve', body: result.weaknesses}, {heading: 'Recommendations', body: result.recommendations}]}}}));
            updateCvAnalysis({type:'selected', documentUri});
            updateCvAnalysis({type:'started', documentUri});
            updateCvAnalysis({type:'progress', documentUri, percent:100});
        }).catch(() => {});
        return () => controller.abort();
    }, [account.token, account.email, cvDocument]);
    useEffect(() => {
        if (!account.token || transcriptDocument) return;
        const controller = new AbortController();
        const owner = account.email.toLowerCase();
        apiRequest<TranscriptAnalysis>('/api/transcripts/current/analysis', {token:account.token,signal:controller.signal}).then(result => {
            if (controller.signal.aborted || !result.analysisID) return;
            const documentUri = `saved:${result.analysisID}`;
            setTranscriptAssessment({...result,documentUri,owner});
            updateTranscriptAnalysis({type:'selected',documentUri});
            updateTranscriptAnalysis({type:'started',documentUri});
            updateTranscriptAnalysis({type:'progress',documentUri,percent:100});
        }).catch(() => { /* No saved report or a temporary failure must not block document upload. */ });
        return () => controller.abort();
    }, [account.token,account.email,transcriptDocument]);
    const [role, setRole] = useState(0);
    const cvSource = useMemo(() => account.token ? cvAnalysisSource(account.token, (result, documentUri) => {
        setCvAssessment({documentUri, role: 0, score: result.qualityScore, atsScore: result.atsScore, formattingScore: result.formattingScore, headline: 'CV analysis',
            summary: result.recommendations, findings: [result.weaknesses].filter(Boolean), analysedAt: result.analysedAt,
            feedback: {sections: [
                {heading: 'Strengths', body: result.strengths},
                {heading: 'Areas to improve', body: result.weaknesses},
                {heading: 'Recommendations', body: result.recommendations},
            ]},
        });
    }) : undefined, [account.token, account.email]);
    const cvRunner = useAnalysisRunner(cvDocument, updateCvAnalysis,cvSource,consent.consent?.status==='granted');
    const transcriptRunner = useAnalysisRunner(transcriptDocument, updateTranscriptAnalysis,transcriptSource,consent.consent?.status==='granted');
    const [query, setQuery] = useState('');
    const [remote, setRemote] = useState(false);
    const cvAssessment=rawCvAssessment&&(!cvDocument ? rawCvAssessment.documentUri.startsWith('saved:') : rawCvAssessment.documentUri===cvDocument.uri)&&(rawCvAssessment.score===null||(Number.isFinite(rawCvAssessment.score)&&rawCvAssessment.score>=0&&rawCvAssessment.score<=100))?rawCvAssessment:null;
    const { careerAssessment, dashboardLoaded: careerAssessmentLoaded, setCareerAssessment } = useCareerAssessment(account.email, account.token, `${rawCvAssessment?.analysedAt ?? ''}:${transcriptAssessment?.analysedAt ?? ''}:${screen === 'home'}`, roles[role] ?? '', role);
    const documentAnalysisEntry = documentEntry(cvAnalysis.status, transcriptAnalysis.status);
    const [job, setJob] = useState('');
    const { savedTopics, toggleSavedTopic, topic, setTopic, currentTopic, checked, setChecked, completed, setCompleted, taskProgress,
        learningGroups, resourcesForTopic, roadmapCareerName, careerSuggestions, generatingCareerID, generateRoadmap,
        learningLoading, learningError, reloadLearning } = useLearningProgress(account.email, account.token, account.targetCareerID);
    const earnedBadgeIds = useCampaignMedals(account.email, account.token, screen === 'achievements', {
        onboardingCompleted: screen === 'home' && cvAnalysis.status === 'completed' && transcriptAnalysis.status === 'completed',
        cvAnalysed: cvAnalysis.status === 'completed',
        opportunitiesExplored: screen === 'job' && !!opportunityModel.selected,
    });
    const scroll = useRef<ScrollView>(null);
    /** Changes the screen and resets its scroll position and error message. */
    const resolveScreen=useCallback((next:Screen):Screen=>next === 'analysis' ? documentEntry(cvAnalysis.status, transcriptAnalysis.status) : next === 'uploadTranscript' && cvAnalysis.status !== 'completed' ? 'upload' : next,[cvAnalysis.status,transcriptAnalysis.status]);
    const updateScreen=useCallback((next:Screen)=>{currentScreen.current=next;setScreen(next);setError('');scroll.current?.scrollTo({y:0,animated:false});},[]);
    const go = useCallback((next:Screen)=>{
        const target=resolveScreen(next),current=currentScreen.current;
        navigationHistory.push(current,target);
        if(target!==current)updateScreen(target);
        else setError('');
    },[navigationHistory,resolveScreen,updateScreen]);
    const replace=useCallback((next:Screen)=>updateScreen(resolveScreen(next)),[resolveScreen,updateScreen]);
    const goBack=useCallback((fallback:Screen)=>updateScreen(navigationHistory.back(currentScreen.current,fallback)),[navigationHistory,updateScreen]);
    const resetTo=useCallback((next:Screen)=>{navigationHistory.clear();updateScreen(resolveScreen(next));},[navigationHistory,resolveScreen,updateScreen]);
    /** Closes the outcome screen and opens its destination. */
    const continueFeedback= ()=>{if(feedback){const next=feedback.next;setFeedback(null);replace(next);}};
    /** Clears the local session and displays the signed-out screen. */
    const signOut= ()=>{
        cvRunner.cancel();transcriptRunner.cancel();mira.clear();setCvDocument(null);setTranscriptDocument(null);setFile('');
        updateCvAnalysis({type:'removed'});updateTranscriptAnalysis({type:'removed'});setCvAssessment(null);setTranscriptAssessment(null);setCareerAssessment(null);
        opportunityModel.setSelected(null);opportunityModel.reset();setLegalDocument(null);setAccount(defaultAccount);setRole(0);
        resetTo('signin');showFeedback('signedOut','signin');
    };
    /** Records withdrawal, stops analysis and displays confirmation. */
    const withdrawConsent= ()=>{
        cvRunner.cancel();transcriptRunner.cancel();mira.clear();
        const saved=consent.withdraw();if(saved)showFeedback('consentWithdrawn','settings');return saved;
    };
    const previousAnalysis=useRef({cv:'idle',transcript:'idle'});
    useEffect(
        /** Opens completion feedback when document analysis finishes. */
         ()=>{
        const previous=previousAnalysis.current;
        if(previous.cv==='processing'&&cvAnalysis.status==='completed')showFeedback('analysisComplete',documentEntry(cvAnalysis.status,transcriptAnalysis.status),transcriptAnalysis.status === 'completed' ? 'Both documents are analysed. Review your results.' : 'Your CV analysis has finished. Continue to step 2 to upload your transcript.');
        else if(previous.transcript==='processing'&&transcriptAnalysis.status==='completed')showFeedback('analysisComplete',documentEntry(cvAnalysis.status,transcriptAnalysis.status),'Both documents have been analysed. Review your CV scores and extracted transcript marks.');
        previousAnalysis.current={cv:cvAnalysis.status,transcript:transcriptAnalysis.status};
    },[cvAnalysis.status,transcriptAnalysis.status]);
    /** Displays an application message on the current platform. */
    const info =  (message: string) => {
        if (Platform.OS === 'web')
            setError(message);
        else
            Alert.alert('Career Path', message);
    };
    /** Opens the document picker, validates the file and stores its selection. */
    const pick =  async (kind: 'cv' | 'transcript' = 'cv') => {
        const transcript = kind === 'transcript';
        const documentLabel = transcript ? 'transcript' : 'CV';
        setError('');
        try {
            const result = await DocumentPicker.getDocumentAsync({ type: ['application/pdf'], copyToCacheDirectory: true });
            if (!result.canceled) {
                const asset = result.assets[0];
                if (!/\.pdf$/i.test(asset.name)) {
                    DocumentPicker.releaseDocument(asset.uri);
                    info(`Choose a PDF ${documentLabel}.`);
                    return;
                }
                if (asset.size === 0) {
                    DocumentPicker.releaseDocument(asset.uri);
                    info(`This file is empty. Choose another ${documentLabel}.`);
                    return;
                }
                if (asset.size && asset.size > 5 * 1024 * 1024) {
                    DocumentPicker.releaseDocument(asset.uri);
                    info(`Choose a ${documentLabel} no larger than 5 MB.`);
                    return;
                }
                if (transcript) {
                    setTranscriptAssessment(null);
                    setTranscriptDocument(asset);
                    updateTranscriptAnalysis({ type: 'selected', documentUri: asset.uri });
                }
                else {
                    setFile(asset.name);
                    setCvDocument(asset);
                    updateCvAnalysis({ type: 'selected', documentUri: asset.uri });
                }
                if (consent.consent?.status !== 'granted') info('Give consent in Settings → Manage consent to start automatic document analysis.');
            }
        }
        catch {
            info('Could not open the document picker. Please try again.');
        }
    };
    const auth = ['signin', 'signup', 'forgot', 'otp', 'reset'].includes(screen);
    const active = ['home', 'analysis', 'jobs', 'learning'].includes(screen) ? screen : screen === 'job' ? 'jobs' : ['topic', 'complete'].includes(screen) ? 'learning' : null;
    const titles: Partial<Record<Screen, string>> = { home: 'Home', analysis: 'CV & Skills', skills: 'CV & Skills', jobs: 'Jobs', job, learning: 'Learning Path', topic: 'Mission Intel', settings: 'Settings' };
    const subtitle = screen === 'home' ? 'Welcome Back' : screen === 'skills' ? 'Analyzed against your selected role' : screen === 'job' ? (opportunityModel.selected?.title === job ? `${opportunityModel.selected.company} · ${opportunityModel.selected.area}, ${opportunityModel.selected.region}` : 'Opportunity details unavailable') : screen === 'learning' ? 'Your learning recommendations' : screen === 'topic' ? topic : '';
    const back = screen === 'job' ? 'jobs' : screen === 'topic' ? 'learning' : screen === 'settings' ? 'home' : undefined;
    const pagePadding = width >= 760 ? 70 : Math.max(20, Math.min(width * 0.077, 40));
    const topicGroup = learningGroups.find(
        /** Checks the group against the current selection or criteria. */
         group => group.topics.includes(topic))?.title || 'Learning path';
    return { documentAnalysisEntry, transcriptAssessment: transcriptAssessment?.owner === account.email.toLowerCase() && (!transcriptDocument || transcriptAssessment.documentUri === transcriptDocument.uri) ? transcriptAssessment : null, feedback,showFeedback,continueFeedback,signOut,consent,withdrawConsent,roles,cvAssessment, setCvAssessment, savedTopics, toggleSavedTopic, earnedBadgeIds, opportunityNotifications, careerAssessment, careerAssessmentLoaded, setCareerAssessment, taskProgress, learningGroups, resourcesForTopic, currentTopic, roadmapCareerName, careerSuggestions, generatingCareerID, generateRoadmap, learningLoading, learningError, reloadLearning, opportunityModel, cvRunner, transcriptRunner, cvAnalysis, updateCvAnalysis, transcriptAnalysis, updateTranscriptAnalysis, mira, legalDocument, openLegal, closeLegal, width, screen, setScreen, account, setAccount, error, setError, file, setFile, cvDocument, setCvDocument, transcriptDocument, setTranscriptDocument, query, setQuery, remote, setRemote, role, setRole, job, setJob, topic, setTopic, checked, setChecked, completed, setCompleted, scroll, go, replace, goBack, resetTo, info, pick, auth, active, titles, subtitle, back, pagePadding, topicGroup };
}
export type AppModel = ReturnType<typeof useAppModel>;
