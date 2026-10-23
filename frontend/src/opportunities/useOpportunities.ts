/** Manages opportunity data, search, filters, saved items and selection. */
import { matchesWorkAreas, type WorkArea } from '../location/types';
import { useEffect, useMemo, useRef, useState, type Dispatch, type SetStateAction } from 'react';
import {apiRequest} from '../api/client';
import { type EmploymentType, type Opportunity, type WorkModel } from './data';
const empty: Opportunity[] = [];
/** Manages opportunity data, search, filters, saved items and selection. */
export default function useOpportunities(profileLocation?: string, account = 'default', workAreas:WorkArea[] = [], token?: string) {
    const [byAccount,setByAccount] = useState<Record<string,Opportunity[]>>({});
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const sessionToken = useRef(token);
    sessionToken.current = token;
    const accountKey = account.toLowerCase();
    const opportunities = byAccount[accountKey] || empty;
    /** Updates the opportunity feed for the current account. */
    const setOpportunities: Dispatch<SetStateAction<Opportunity[]>> =  action => setByAccount(
        /** Builds the next by account value from its previous state. */
         old => ({...old,[accountKey]:typeof action === 'function' ? action(old[accountKey] || empty) : action}));
    const [query, setQuery] = useState('');
    const [workModel, setWorkModel] = useState<WorkModel | 'All'>('All');
    const [employmentType, setEmploymentType] = useState<EmploymentType | 'All'>('All');
    const [locationOnly, setLocationOnly] = useState(false);
    const location = profileLocation?.trim() || '';
    const [saved, setSaved] = useState<string[]>([]);
    const [savedOnly, setSavedOnly] = useState(false);
    const [selected, setSelected] = useState<Opportunity | null>(null);
    useEffect(() => {
        if (!token) { setSaved([]); return; }
        const controller = new AbortController();
        const signal = controller.signal;
        setLoading(true); setError(''); setSaved([]);
        const load = async () => {
            try {
                const items: JobSummary[] = [];
                let page = 1, totalPages = 1;
                do {
                    const result = await apiRequest<{items: JobSummary[]; totalPages: number}>(`/api/jobs?page=${page}&pageSize=50`, {signal});
                    items.push(...result.items); totalPages = result.totalPages; page++;
                } while (page <= totalPages && !signal.aborted);
                if (!signal.aborted) setOpportunities(items.map(toOpportunity));
                const matches = await apiRequest<{jobID: string; matchPercentage: number; matchedSkills: {skillName: string}[]; missingSkills: {skillName: string}[]}[]>('/api/job-matches', {token, signal});
                const byId = new Map(matches.map(match => [match.jobID, match]));
                if (!signal.aborted) setOpportunities(items.map(job => {
                    const match = byId.get(job.jobID);
                    return {...toOpportunity(job), match: match?.matchPercentage ?? null, requiredSkills: match ? [
                        ...match.matchedSkills.map(skill => ({name: skill.skillName, status: 'matched' as const})),
                        ...match.missingSkills.map(skill => ({name: skill.skillName, status: 'missing' as const})),
                    ] : undefined};
                }));
            } catch (cause) { if (!signal.aborted) setError(cause instanceof Error ? cause.message : 'Could not load jobs.'); }
            finally { if (!signal.aborted) setLoading(false); }
        };
        void load();
        apiRequest<{job: JobSummary}[]>('/api/saved-jobs', {token, signal}).then(items => {
            if (!signal.aborted) setSaved(items.map(item => item.job.jobID));
        }).catch(cause => { if (!signal.aborted) setError(cause instanceof Error ? cause.message : 'Could not load saved jobs.'); });
        return () => controller.abort();
    }, [accountKey, token]);
    useEffect(() => {
        if (!selected) return;
        const controller = new AbortController();
        apiRequest<JobSummary & {description: string | null; applicationUrl: string | null}>(`/api/jobs/${selected.id}`, {signal: controller.signal}).then(job => {
            if (!controller.signal.aborted) setSelected(current => current?.id === job.jobID ? {...current, description: job.description || '', applicationUrl: job.applicationUrl || undefined} : current);
        }).catch(cause => { if (!controller.signal.aborted) setError(cause instanceof Error ? cause.message : 'Could not load job details.'); });
        return () => controller.abort();
    }, [selected?.id]);
    /** Filters and sorts opportunities using the current search and preferences. */
    const results = useMemo( () => opportunities.filter(
        /** Selects the opportunities that match the current conditions. */
         item => {
        const text = `${item.title} ${item.company} ${item.area} ${item.region}`.toLowerCase();
        return text.includes(query.trim().toLowerCase()) && (workModel === 'All' || item.workModel === workModel) && (employmentType === 'All' || item.employmentType === employmentType) && (!locationOnly || (matchesWorkAreas(item.area,item.region,workAreas,location))) && (!savedOnly || saved.includes(item.id));
    }).sort(
        /** Compares values to determine their displayed order. */
         (a, b) => (b.match ?? -1) - (a.match ?? -1)), [opportunities, query, workModel, employmentType, location, workAreas, locationOnly, savedOnly, saved]);
    const saving = useRef(new Set<string>());
    const toggleSaved = async (id: string) => {
        if (!token || saving.current.has(id)) return;
        saving.current.add(id);
        try {
            const remove = saved.includes(id);
            await apiRequest(`/api/saved-jobs/${id}`, {method: remove ? 'DELETE' : 'POST', token});
            if (sessionToken.current !== token) return;
            setSaved(items => remove ? items.filter(item => item !== id) : [...new Set([...items, id])]);
            setError('');
        } catch (cause) { if (sessionToken.current === token) setError(cause instanceof Error ? cause.message : 'Could not save job.'); }
        finally { saving.current.delete(id); }
    };
    /** Clears search and filter selections. */
    const reset =  () => { setQuery(''); setWorkModel('All'); setEmploymentType('All'); setLocationOnly(false); setSavedOnly(false); };
    return { error, loading, allOpportunities: opportunities, setOpportunities, locationOnly, setLocationOnly, query, setQuery, workModel, setWorkModel, employmentType, setEmploymentType, location, saved, savedOnly, setSavedOnly, selected, setSelected, results, toggleSaved, reset };
}

type JobSummary = {jobID: string; jobTitle: string; companyName: string; location: string | null; jobType: string | null};
function toOpportunity(job: JobSummary): Opportunity {
    const kind = job.jobType?.toLowerCase();
    return {id: job.jobID, title: job.jobTitle, company: job.companyName,
        area: job.location || '', region: '', description: '', match: null,
        workModel: 'Unspecified', employmentType: kind === 'full-time' ? 'Full-time' : kind === 'part-time' ? 'Part-time' : kind === 'contract' ? 'Contract' : 'Unspecified'};
}
