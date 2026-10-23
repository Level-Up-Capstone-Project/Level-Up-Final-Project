/** Stores career assessment results separately for each account. */
import { useEffect, useState } from 'react';
import {apiRequest} from '../api/client';
export type CareerAssessment = {
    skillMatch?: {role:number; overall:number; summary:string; skills:{name:string;score:number;status:string}[]};
    readiness: number | null;
    cvQualityScore?: number | null;
    trackedSkillCount?: number;
    targetCareer?: string | null;
    skills: {name: string;score: number;level: string}[];
    prioritySkills: string[];
    readinessChangeThisMonth: number | null;
    onTrack: boolean | null;
};
/** Stores career assessment results separately for each account. */
export default function useCareerAssessment(account: string, token?: string, revision = '', targetRole = '', roleIndex = 0) {
    const [byAccount,setByAccount] = useState<Record<string,CareerAssessment | null>>({});
    const [loadedAccounts,setLoadedAccounts] = useState<Record<string,boolean>>({});
    const key = account.toLowerCase();
    const careerAssessment = key in byAccount ? byAccount[key] : null;
    const dashboardLoaded = loadedAccounts[key] ?? false;
    useEffect(() => {
        if (!token) return;
        const controller = new AbortController();
        const options = {token, signal: controller.signal};
        const dashboardRequest = async () => {
            const loadDashboard = () => apiRequest<{careerReadinessScore: number | null; cvQualityScore: number | null; targetCareer: {careerName: string} | null}>('/api/dashboard', options);
            let dashboard = await loadDashboard();
            if (targetRole.trim() && dashboard.targetCareer?.careerName.toLowerCase() !== targetRole.trim().toLowerCase()) {
                const careers = await apiRequest<{careerID: string; careerName: string}[]>('/api/careers', options);
                const career = careers.find(item => item.careerName.trim().toLowerCase() === targetRole.trim().toLowerCase());
                if (career) {
                    await apiRequest('/api/profile/target-career', {...options, method: 'PUT', body: {careerID: career.careerID}});
                    dashboard = await loadDashboard();
                }
            }
            return dashboard;
        };
        const loadAssessment = async () => {
            const dashboard = await Promise.allSettled([dashboardRequest()]);
            const profileSkills = await Promise.allSettled([
                apiRequest<{skillID:string;skillName:string;status:string}[]>('/api/profile/skills', options),
            ]);
            const skillGap = await Promise.allSettled([
                apiRequest<{career:{careerName:string};skillMatchPercentage:number;skills:{skillName:string;status:string}[]}>('/api/skill-gap', options),
            ]);
            if (controller.signal.aborted) return;
            const statusScore = (status:string) => status === 'Have' ? 100 : status === 'Partial' ? 50 : 0;
            const gap = skillGap[0].status === 'fulfilled' ? skillGap[0].value : null;
            const trackedSkills = profileSkills[0].status === 'fulfilled' ? profileSkills[0].value : [];
            const skills = gap
                ? gap.skills.map(skill => ({name:skill.skillName, score:statusScore(skill.status), level:skill.status}))
                : trackedSkills.map(skill => ({name:skill.skillName, score:statusScore(skill.status), level:skill.status}));
            setByAccount(old => {
                const previous = old[key];
                const career = dashboard[0].status === 'fulfilled' ? dashboard[0].value.targetCareer?.careerName ?? null : previous?.targetCareer ?? null;
                return {...old, [key]: {
                    skills,
                    skillMatch: gap ? {role:roleIndex, overall:gap.skillMatchPercentage,
                        summary:`Skill match for ${gap.career.careerName}, based on your current skill evidence.`, skills:skills.map(({name,score,level})=>({name,score,status:level}))}
                        : undefined,
                    prioritySkills: previous?.prioritySkills ?? [], readinessChangeThisMonth: previous?.readinessChangeThisMonth ?? null, onTrack: previous?.onTrack ?? null,
                    readiness: dashboard[0].status === 'fulfilled' ? dashboard[0].value.careerReadinessScore : previous?.readiness ?? null,
                    cvQualityScore: dashboard[0].status === 'fulfilled' ? dashboard[0].value.cvQualityScore : previous?.cvQualityScore,
                    targetCareer: gap?.career.careerName ?? career,
                    trackedSkillCount: profileSkills[0].status === 'fulfilled' ? new Set(profileSkills[0].value.map(skill => skill.skillID)).size : previous?.trackedSkillCount,
                }};
            });
            setLoadedAccounts(old=>({...old,[key]:true}));
        };
        void loadAssessment();
        return () => controller.abort();
    }, [key, token, revision, targetRole, roleIndex]);
    /** Stores assessment results for the current account. */
    const setCareerAssessment =  (assessment: CareerAssessment | null) => setByAccount(
        /** Builds the next by account value from its previous state. */
         old => ({...old,[key]:assessment}));
    return {careerAssessment,dashboardLoaded,setCareerAssessment};
}
