/** Defines document-analysis stages and updates their progress state. */
export type AnalysisTaskStatus = 'upcoming' | 'processing' | 'completed' | 'failed';
export type AnalysisStatus = 'idle' | 'ready' | 'processing' | 'completed' | 'failed';
export type AnalysisState = {
    documentUri: string | null;
    status: AnalysisStatus;
    tasks: AnalysisTaskStatus[];
    taskProgress: number[];
    error: string | null;
};
export type AnalysisEvent =
    | { type: 'selected'; documentUri: string }
    | { type: 'removed' }
    | { type: 'started'; documentUri: string }
    | { type: 'progress'; documentUri: string; percent: number }
    | { type: 'failed'; documentUri: string; error: string }
    | { type: 'task'; documentUri: string; index: number; status: AnalysisTaskStatus; progress?: number; error?: string };

/** Creates the initial document-analysis state. */
export function initialAnalysisState(): AnalysisState {
    return { documentUri: null, status: 'idle', tasks: ['upcoming', 'upcoming', 'upcoming', 'upcoming'], taskProgress: [0, 0, 0, 0], error: null };
}

// Processing integrations dispatch events for the selected document. Late events
// from a removed/replaced document cannot complete the new document's analysis.
/** Applies document selection, progress, task and failure events to analysis state. */
export function analysisReducer(state: AnalysisState, event: AnalysisEvent): AnalysisState {
    if (event.type === 'removed') return initialAnalysisState();
    if (event.type === 'selected') return { ...initialAnalysisState(), documentUri: event.documentUri, status: 'ready' };
    if (event.documentUri !== state.documentUri) return state;
    if (event.type === 'started') return { ...state, status: 'processing', tasks: ['processing', 'upcoming', 'upcoming', 'upcoming'], taskProgress: [0, 0, 0, 0], error: null };
    if (event.type === 'failed') {
        if (state.status !== 'processing') return state;
        /** Finds the first analysis stage that is not completed. */
        const index = state.tasks.findIndex( task => task !== 'completed');
        return { ...state, status: 'failed', error: event.error, tasks: state.tasks.map(
            /** Transforms each item into the required display or state value. */
             (task, i) => i === index ? 'failed' : task) };
    }
    if (event.type === 'progress') {
        if (state.status !== 'processing' || !Number.isFinite(event.percent)) return state;
        const percent = Math.max(0, Math.min(100, event.percent));
        const previous = state.taskProgress.reduce(
            /** Accumulates the values used for the current total or summary. */
             (sum, value) => sum + value, 0) / state.tasks.length;
        if (percent < previous) return state;
        /** Calculates the updated progress for an analysis stage. */
        const taskProgress = state.tasks.map( (_, index) => Math.max(0, Math.min(100, percent * state.tasks.length - index * 100)));
        /** Determines the updated state of each analysis stage. */
        const tasks: AnalysisTaskStatus[] = taskProgress.map( (value, index) => value === 100 ? 'completed' : index === 0 || taskProgress[index - 1] === 100 ? 'processing' : 'upcoming');
        return { ...state, taskProgress, tasks, status: percent === 100 ? 'completed' : 'processing', error: null };
    }
    if (state.status !== 'processing' || event.index < 0 || event.index >= state.tasks.length || !Number.isInteger(event.index)) return state;
    if (state.tasks[event.index] === 'completed') return state;
    if (event.progress !== undefined && !Number.isFinite(event.progress)) return state;
    /** Calculates the updated progress for an analysis stage. */
    const taskProgress = state.taskProgress.map( (value, index) => index !== event.index ? value : event.status === 'completed' ? 100 : Math.max(value, Math.min(100, Math.max(0, event.progress ?? value))));
    /** Determines the updated state of each analysis stage. */
    const tasks = state.tasks.map( (task, index) => index === event.index ? (event.status === 'failed' ? 'failed' : taskProgress[index] === 100 ? 'completed' : event.status) : task);
    /** Moves to the next introductory screen or sign-in. */
    const next = tasks.findIndex( task => task !== 'completed');
    if (next >= 0 && tasks[next] === 'upcoming') tasks[next] = 'processing';
    const failed = tasks.includes('failed');
    /** Checks whether an analysis stage has completed. */
    const complete = tasks.every( task => task === 'completed');
    return { ...state, tasks, taskProgress, status: failed ? 'failed' : complete ? 'completed' : 'processing', error: failed ? event.error || 'Analysis failed. Please try again.' : null };
}

/** Calculates overall progress from the analysis stages. */
export function analysisProgress(state: AnalysisState): number {
    const percent = Math.round(state.taskProgress.reduce(
        /** Accumulates the values used for the current total or summary. */
         (sum, value) => sum + value, 0) / state.tasks.length);
    return state.status === 'completed' ? 100 : Math.min(99, percent);
}
