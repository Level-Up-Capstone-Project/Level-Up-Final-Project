import {useEffect, useState} from 'react';
import {apiRequest} from '../api/client';
import {earnedMedals, type MedalEvidence} from './eligibility';

type Dashboard = {profile: {profileCompleted: boolean} | null; targetCareer: {careerID: string} | null; careerReadinessScore: number | null};
type Observed = {onboardingCompleted: boolean; cvAnalysed: boolean; opportunitiesExplored: boolean};

export default function useCampaignMedals(email: string, token: string | undefined, open: boolean, observed: Observed) {
    const owner = email.toLowerCase();
    const [records, setRecords] = useState<Record<string, MedalEvidence>>({});
    useEffect(() => {
        if (!token || !owner) return;
        if (!observed.onboardingCompleted && !observed.cvAnalysed && !observed.opportunitiesExplored) return;
        setRecords(old => ({...old, [owner]: {...old[owner],
            onboardingCompleted: old[owner]?.onboardingCompleted || observed.onboardingCompleted,
            cvAnalysed: old[owner]?.cvAnalysed || observed.cvAnalysed,
            opportunitiesExplored: old[owner]?.opportunitiesExplored || observed.opportunitiesExplored,
        }}));
    }, [owner, token, observed.onboardingCompleted, observed.cvAnalysed, observed.opportunitiesExplored]);
    useEffect(() => {
        if (!token || !open) return;
        const controller = new AbortController();
        const options = {token, signal: controller.signal};
        void Promise.allSettled([
            apiRequest<Dashboard>('/api/dashboard', options),
            apiRequest<{analysisID: string}>('/api/cvs/current/analysis', options),
            apiRequest<{isKnowledgeVerified: boolean}[]>('/api/profile/skills', options),
            apiRequest<{progressPercentage: number}[]>('/api/roadmaps', options),
        ]).then(([dashboard, cvAnalysis, skills, roadmaps]) => {
            if (controller.signal.aborted) return;
            setRecords(old => {
                const next = {...old[owner]};
                if (dashboard.status === 'fulfilled') {
                    next.profileCompleted = dashboard.value.profile?.profileCompleted === true;
                    next.targetCareerSelected = !!dashboard.value.targetCareer?.careerID;
                    next.readiness = dashboard.value.careerReadinessScore;
                }
                if (cvAnalysis.status === 'fulfilled') next.cvAnalysed = true;
                if (skills.status === 'fulfilled') next.verifiedSkills = skills.value.filter(skill => skill.isKnowledgeVerified).length;
                if (roadmaps.status === 'fulfilled') {
                    next.roadmapCreated = roadmaps.value.length > 0;
                    next.roadmapStarted = roadmaps.value.some(roadmap => roadmap.progressPercentage > 0);
                    next.roadmapCompleted = roadmaps.value.some(roadmap => roadmap.progressPercentage === 100);
                }
                return {...old, [owner]: next};
            });
        });
        return () => controller.abort();
    }, [owner, token, open]);
    return token ? earnedMedals(records[owner] ?? {}) : [];
}
