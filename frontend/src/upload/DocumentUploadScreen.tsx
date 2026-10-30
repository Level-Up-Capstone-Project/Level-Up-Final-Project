/** Arranges document selection, file details and analysis progress controls. */
import ScreenHeader, {screenHeaderTop} from '../components/ScreenHeader';
import VignettePair from '../components/VignettePair';
import { useEffect, useState } from 'react';
import { BackHandler, Image, Platform, Pressable, ScrollView, StyleSheet, useWindowDimensions, View, type ImageSourcePropType } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import Svg, { Defs, LinearGradient, Rect, Stop } from 'react-native-svg';
import Text from '../components/Text';
import OpportunityIcon from '../opportunities/OpportunityIcon';
import CircularProgress from './CircularProgress';
import StatusNode from './StatusNode';
import FileTypeIcon from './FileTypeIcon';
import UploadVector from './UploadVector';
import { analysisProgress } from './analysisState';
import type { AppModel } from '../navigation/useAppModel';

const cvTasks = ['Uploading document', 'Reading text / OCR', 'Analysing CV and skills', 'Saving analysis results'];
const transcriptTasks = ['Validating document', 'Reading with Adobe OCR', 'Analysing transcript with AI', 'Saving your report'];

/** Arranges document selection, file details and analysis progress controls. */
export default function DocumentUploadScreen({ model, kind }: { model: AppModel; kind: 'cv' | 'transcript' }) {
    const transcript = kind === 'transcript';
    const label = transcript ? 'transcript' : 'CV';
    const tasks = transcript ? transcriptTasks : cvTasks;
    const { width, height } = useWindowDimensions();
    const insets = useSafeAreaInsets();
    const scale = width / 408.393;
    const [choosing, setChoosing] = useState(false);
    const { error, go, goBack } = model;
    const document = transcript ? model.transcriptDocument : model.cvDocument;
    const analysis = transcript ? model.transcriptAnalysis : model.cvAnalysis;
    const updateAnalysis = transcript ? model.updateTranscriptAnalysis : model.updateCvAnalysis;
    const file = transcript ? document?.name || '' : model.file;
    const backScreen = transcript ? 'upload' : 'home';
    useEffect(
        /** Registers the hardware back action and its cleanup. */
         () => {
        /** Handles the device back action for this screen. */
        if (Platform.OS === 'web') return;
        const subscription = BackHandler.addEventListener('hardwareBackPress',  () => { goBack(backScreen); return true; });
        /** Removes the registered listeners when the effect ends. */
        return  () => subscription.remove();
    }, [goBack, backScreen]);
    /** Opens the document selector and tracks whether it is active. */
    const chooseFile =  async () => {
        if (choosing) return;
        setChoosing(true);
        try { await model.pick(kind); } finally { setChoosing(false); }
    };
    /** Clears the selected document and its analysis state. */
    const removeFile =  () => {
        if (transcript) model.setTranscriptDocument(null);
        else { model.setFile(''); model.setCvDocument(null); }
        updateAnalysis({ type: 'removed' });
        model.setError('');
    };
    const runner = transcript ? model.transcriptRunner : model.cvRunner;
    const processing = analysis.status === 'processing';
    const progress = analysisProgress(analysis);
    const ownershipMismatch = analysis.status === 'failed' && analysis.error?.startsWith('Name mismatch:') === true;
    /** Displays a bundled image in its upload-screen position. */
    const photo =  (source: ImageSourcePropType, left: number, top: number, w: number, h = w) => <Image accessible={false} source={source} resizeMode="cover" style={{ position: 'absolute', left, top, width: w, height: h }}/>;
    const fileSize = document?.size == null ? 'Size unavailable' : document.size < 1024 * 1024 ? `${Math.max(1, Math.round(document.size / 1024))} KB` : `${(document.size / (1024 * 1024)).toFixed(2)} MB`;
    // Keep the existing Continue action available after automatic processing succeeds.
    const canContinue = ownershipMismatch || (!!document && (runner.connected ? analysis.status === 'completed' : ['ready', 'completed'].includes(analysis.status)) && !choosing);
    return <View style={styles.safe}>
        <ScrollView bounces={false} contentContainerStyle={{ flexGrow: 1, alignItems: 'center' }}>
            <View testID={transcript ? 'upload-transcript-screen' : 'upload-screen'} style={{ width: 408.393 * scale, height: Math.max(height, (885.372 + insets.bottom) * scale), overflow: 'hidden' }}>
                <View style={{ width: 408.393, height: 885.372, transformOrigin: 'top left', transform: [{ scale }] }}>
                    {photo(transcript ? {uri:'/assets/upload/92fd0.png'} : {uri:'/assets/upload/d9e27.png'}, -38.78, .22, 486, 274)}
                    {transcript && photo({uri:'/assets/upload/9eab2.png'}, -16, -5, 436, 244)}
                    <VignettePair/>

                    <View style={{position:'absolute',left:23,right:23,top:screenHeaderTop(scale,insets.top)/scale,zIndex:2}}><ScreenHeader title={transcript?'UPLOAD TRANSCRIPT':'UPLOAD CV'} onBack={
                        /** Handles the on back for this component. */
                         ()=>goBack(backScreen)} backLabel={transcript?'Back to CV upload':'Back to previous page'}/></View>
                    <Text style={[styles.small, { left: 53, top: 132, width: 202 }]}>{transcript ? 'Upload your transcript and let us analyze your subjects, marks and learning opportunities.' : 'Upload your CV and let us analyze your skills, experience and career experience opportunities.'}</Text>
                    <View accessibilityLabel={transcript ? 'Step 2 of 2' : 'Step 1 of 2'} style={styles.stepper}>{[0, 1].map(
                        /** Renders a item entry from its data. */
                         step => <View key={step} style={[styles.step, step === (transcript ? 1 : 0) && { backgroundColor: '#e9b764' }]}/>)}</View>
                    <View style={[styles.card, styles.upload]}>
                        {photo({uri:'/assets/upload/e8b00.png'}, 150.22, 12, 58)}
                        <Text style={[styles.bold, { top: 73, textAlign: 'center', width: '100%' }]}>Upload your {label} here</Text>
                        <Pressable accessibilityRole="button" accessibilityLabel={`Choose ${label} file`} accessibilityState={{ disabled: choosing, busy: choosing }} disabled={choosing} onPress={chooseFile} style={
                            /** Returns the control styles for its current pressed state. */
                             ({ pressed }) => [styles.choose, { opacity: pressed || choosing ? .75 : 1 }]}>
                            <Svg pointerEvents="none" width={191} height={42} style={StyleSheet.absoluteFill}><Defs><LinearGradient id="upload-gold" x1="0" y1="0" x2="0" y2="1"><Stop offset="0" stopColor="#dfa94a"/><Stop offset="0.5" stopColor="#c98c30"/><Stop offset="1" stopColor="#a86e1c"/></LinearGradient></Defs><Rect width={191} height={42} rx={10} fill="url(#upload-gold)"/></Svg>
                            <View style={{ transform: [{ scaleX: -1 }] }}><UploadVector source={{uri:'/assets/upload/246dc.svg'}} width={14}/></View><Text style={styles.buttonText}>{choosing ? 'Opening…' : 'Choose File'}</Text>
                        </Pressable>
                    </View>
                    <View style={[styles.card, { top: 386, height: 66 }]}>
                        {photo({uri:'/assets/upload/4110e.png'}, 11, 17, 31)}
                        <Text style={[styles.bold, { left: 51, top: 8 }]}>Supported formats</Text>
                        <Text style={[styles.small, { left: 51, top: 24 }]}>{'PDF'}{'\n'}(5MB Max)</Text>
                        <View style={{ position: 'absolute', left: 182, top: 8, transformOrigin: 'top left', transform: [{ rotate: '90deg' }] }}><UploadVector source={{uri:'/assets/upload/85e81.svg'}} width={47} height={1}/></View>
                        {photo({uri:'/assets/upload/21926.png'}, 193, 17, 31)}
                        <Text style={[styles.bold, { left: 234, top: 8 }]}>Your data is secure</Text>
                        <Text style={[styles.small, { left: 234, top: 24 }]}>We keep your information{'\n'}private and safe.</Text>
                    </View>
                    <View style={[styles.card, { top: 467, height: 66 }]}>
                        <View style={{ position: 'absolute', left: 10, top: 8 }}>{transcript ? <FileTypeIcon filename="transcript.pdf"/> : document ? <FileTypeIcon filename={file}/> : photo({uri:'/assets/upload/94f58.png'}, 7, 5, 40, 39)}</View>
                        <Text numberOfLines={1} style={[styles.bold, { left: 76, top: 17, width: 230 }]}>{file || `No ${label} selected`}</Text>
                        <Text style={[styles.small, { left: 76, top: 33 }]}>{document ? fileSize : 'Choose a file to get started'}</Text>
                        {!!file && <Pressable accessibilityRole="button" accessibilityLabel={`Remove ${label}`} onPress={removeFile} style={styles.trash}><UploadVector source={{uri:'/assets/upload/8b2c6.svg'}} width={17}/></Pressable>}
                    </View>
                    <Pressable testID={`${kind}-analysis-card`} accessibilityRole="button" accessibilityLabel={processing ? 'Cancel analysis' : runner.connected ? 'Retry analysis' : 'Analysis unavailable'} accessibilityHint={runner.connected ? 'Analysis starts automatically. Activate to cancel or retry.' : 'The analysis service is not connected.'} accessibilityValue={{ min: 0, max: 100, now: progress, text: `${progress}%` }} accessibilityState={{ disabled: !document || !runner.connected || analysis.status === 'completed', busy: processing }} disabled={!document || !runner.connected || analysis.status === 'completed'} onPress={
                        /** Starts or cancels document analysis for the selected file. */
                         () => processing ? runner.cancel() : void runner.start()} style={[styles.card, { top: 546, height: 135 }]}>
                        <View style={{ position: 'absolute', left: 25, top: 22 }}><CircularProgress progress={progress} gold label={`${label} analysis`}/></View>
                        <Text accessibilityLiveRegion="polite" style={[styles.bold, { left: 154, top: 10 }]}>{analysis.status === 'failed' ? 'Analysis could not finish' : analysis.status === 'completed' ? `Your ${label} is ready` : processing ? `Analyzing your ${label}...` : document ? (runner.connected ? model.consent.consent?.status === 'granted' ? 'Starting analysis...' : 'Consent required in Settings' : 'Analysis service unavailable') : `Ready to analyze your ${label}`}</Text>
                        {tasks.map(
                            /** Renders a item entry from its data. */
                             (task, index) => <View key={task} accessibilityLabel={`${task}: ${analysis.tasks[index]}`} style={{ position: 'absolute', left: 153, top: 37 + index * 22, flexDirection: 'row', alignItems: 'center', gap: 13 }}><StatusNode gold state={analysis.tasks[index]} size={15.9091}/><Text style={styles.task}>{task}</Text></View>)}
                    </Pressable>
                    <View style={[styles.card, styles.personalize]}>
                        {photo({uri:'/assets/upload/8701f.png'}, 10, 5, 63)}
                        <Text style={[styles.bold, { left: 82, top: 13 }]}>Personalizing your results</Text>
                        <Text style={[styles.small, { left: 82, top: 29, width: 264 }]}>{transcript ? model.transcriptAssessment?.summary || 'Your transcript is read automatically to extract subject marks and skill terms.' : 'We will use your CV to recommend jobs, skills to improve,\nand learning opportunities tailored to your goals.'}</Text>
                    </View>
                    {!!(error || analysis.error) && <Text accessibilityRole="alert" style={styles.error}>{error || analysis.error}</Text>}
                    <Pressable accessibilityRole="button" accessibilityState={{ disabled: !canContinue }} disabled={!canContinue} onPress={
                        /** Handles the on press for this component. */
                         () => ownershipMismatch ? go('profileSettings') : go(transcript ? 'analysis' : 'uploadTranscript')} style={
                        /** Returns the control styles for its current pressed state. */
                         ({ pressed }) => [styles.continue, canContinue && pressed && { opacity: .8 }]}><Text style={styles.buttonText}>{ownershipMismatch ? 'Update profile' : 'Continue'}</Text><OpportunityIcon name="arrow" size={24} color="#000"/></Pressable>
                </View>
            </View>
        </ScrollView>
    </View>;
}

const styles = StyleSheet.create({
    safe: { flex: 1, backgroundColor: '#000' },
    back: { position: 'absolute', left: 18, top: 80, width: 44, height: 44, alignItems: 'center', justifyContent: 'center' },
    title: { position: 'absolute', left: 60, top: 88, fontSize: 30, fontWeight: '600', letterSpacing: -.8, color: '#fff' },
    small: { position: 'absolute', fontFamily: Platform.select({ web: 'Arial', android: 'Roboto', ios: 'System' }), fontSize: 8, lineHeight: 16, letterSpacing: .4, color: '#d1d1d6' },
    bold: { position: 'absolute', fontSize: 12, lineHeight: 16, fontWeight: '700', letterSpacing: .12, color: '#fff' },
    stepper: { position: 'absolute', left: 53, top: 180, width: 298, flexDirection: 'row', gap: 4 },
    step: { flex: 1, height: 4, borderRadius: 100, backgroundColor: '#c1c9d2' },
    card: { position: 'absolute', left: 23, width: 363, backgroundColor: 'rgba(6,15,20,.78)', borderColor: '#353e44', borderWidth: 1, borderRadius: 10 },
    upload: { top: 211, height: 161, borderColor: '#d4af53', borderStyle: 'dashed' },
    choose: { position: 'absolute', left: 86, top: 99, width: 191, height: 42, borderRadius: 10, flexDirection: 'row', alignItems: 'center', justifyContent: 'center', gap: 16, boxShadow: '0 9px 22px rgba(201,140,48,.28)' },
    buttonText: { fontFamily: 'PoppinsMedium', fontSize: 16, lineHeight: 24, color: '#000' },
    trash: { position: 'absolute', right: 1, top: 10, width: 44, height: 44, alignItems: 'center', justifyContent: 'center' },
    task: { fontFamily: Platform.select({ web: 'Arial', android: 'Roboto', ios: 'System' }), fontSize: 10, lineHeight: 22, letterSpacing: .4, color: '#d1d1d6' },
    personalize: { top: 696, height: 74, backgroundColor: '#252112', borderColor: '#765728' },
    continue: { position: 'absolute', left: 23, top: 790, width: 363, height: 54, borderRadius: 10, backgroundColor: '#c4872d', borderWidth: 1, borderColor: '#ba7e27', overflow: 'hidden', alignItems: 'center', justifyContent: 'center', flexDirection:'row',gap:16 },
    error: { position: 'absolute', left: 23, top: 772, color: '#ffb4ab', fontSize: 10, lineHeight: 16 },
});
