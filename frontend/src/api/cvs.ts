import {apiRequest} from './client';
import type {AnalysisSource} from '../upload/useAnalysisRunner';

export type CVAnalysis = {
    analysisID?: string;
    cvid?: string;
    qualityScore: number; atsScore: number; formattingScore: number;
    strengths: string; weaknesses: string; recommendations: string; analysedAt: string;
};

/** Uploads the selected browser file before asking the backend to analyse it. */
export function cvAnalysisSource(token: string, onResult: (result: CVAnalysis, uri: string) => void): AnalysisSource {
    let uploaded: {uri: string; cvid: string} | null = null;
    return async ({document, signal, onProgress}) => {
        if (uploaded?.uri !== document.uri) {
            const fileResponse = await fetch(document.uri, {signal});
            if (!fileResponse.ok) throw new Error('Could not read the selected file. Please select it again.');
            const blob = await fileResponse.blob();
            const mimeType = 'application/pdf';
            const form = new FormData();
            form.append('file', new Blob([blob], {type: mimeType}), document.name);
            onProgress(5);
            const cv = await apiRequest<{cvid: string}>('/api/cvs', {method: 'POST', body: form, token, signal});
            uploaded = {uri: document.uri, cvid: cv.cvid};
        }
        onProgress(20);
        const result = await apiRequest<CVAnalysis>(`/api/cvs/${uploaded.cvid}/analyse`, {method: 'POST', token, signal, onProgress: percent => onProgress(20 + percent * .8)});
        if (!signal.aborted) onResult(result, document.uri);
    };
}
