import {apiRequest} from './client';
import type {AnalysisSource} from '../upload/useAnalysisRunner';

export type TranscriptAnalysis = {
    analysedAt: string; summary: string; textCharacters: number;
    averageMark: number | null;
    analysisID?: string; fileName?: string; fileSize?: number; careerName?: string | null;
    academicScore?: number | null; institution?: string | null; qualification?: string | null;
    modules?: {moduleName: string; mark: number | null; evidence: string}[];
    skillEvidence?: {skillName: string; evidence: string}[];
    strengths?: string[]; provider?: string;
    subjects: {subject: string; mark: number}[];
    detectedSkills: {skillID: string; skillName: string}[];
};

export function transcriptAnalysisSource(token: string, onResult: (result: TranscriptAnalysis, uri: string) => void): AnalysisSource {
    return async ({document, signal, onProgress}) => {
        const response = await fetch(document.uri, {signal});
        if (!response.ok) throw new Error('Could not read the selected transcript. Select it again.');
        const file = await response.blob();
        const form = new FormData();
        form.append('file', new Blob([file], {type: 'application/pdf'}), document.name);
        onProgress?.(5);
        const result = await apiRequest<TranscriptAnalysis>('/api/transcripts/analyse', {method: 'POST', body: form, token, signal, onProgress: percent => onProgress?.(5 + percent * .95)});
        if (!signal.aborted) onResult(result, document.uri);
    };
}
