import type {AnalysisStatus} from './analysisState';

/** CV and transcript are two required stages of the existing document flow. */
export function documentEntry(cv: AnalysisStatus, transcript: AnalysisStatus): 'upload' | 'uploadTranscript' | 'analysis' {
    return cv !== 'completed' ? 'upload' : transcript !== 'completed' ? 'uploadTranscript' : 'analysis';
}
