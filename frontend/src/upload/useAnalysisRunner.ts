/** Runs an analysis source and coordinates progress, cancellation and errors. */
import { useCallback, useEffect, useRef, useState, type Dispatch } from 'react';
import type { AnalysisEvent } from './analysisState';

export type AnalysisDocument = { uri: string; name: string; mimeType?: string; size?: number };
export type AnalysisSource = (request: {
    document: AnalysisDocument;
    signal: AbortSignal;
    // Real adapters report server progress here and resolve only on success.
    onProgress: (percent: number) => void;
}) => Promise<void>;

/** Turns the protected-document response into a clear, actionable user message. */
function analysisErrorMessage(error: unknown): string {
    if (typeof error === 'object' && error !== null && 'status' in error &&
        (error as {status?: unknown}).status === 403)
        return 'Name mismatch: this document must show the same first name and surname as your Level Up profile. Update your profile to match the name on your ID, CV or institution record, then try again.';
    return error instanceof Error ? error.message : 'Analysis could not finish. Try again.';
}

/** Runs an analysis source and coordinates progress, cancellation and errors. */
export default function useAnalysisRunner(document: AnalysisDocument | null, dispatch: Dispatch<AnalysisEvent>, source?: AnalysisSource, enabled=true) {
    const current = useRef<{ controller: AbortController; uri: string } | null>(null);
    const attempted = useRef(false);
    const [mode, setMode] = useState<'live' | null>(null);
    useEffect(
        /** Cancels analysis when the document or consent state changes. */
         () => {
        current.current?.controller.abort();
        current.current = null;
        attempted.current = false;
        setMode(null);
        /** Cancels outstanding work when the effect ends. */
        return  () => { current.current?.controller.abort(); current.current = null; };
    }, [document, source]);
    /** Cancels the current analysis run and restores the selected-file state. */
    const cancel = useCallback( () => {
        current.current?.controller.abort();
        current.current = null;
        setMode(null);
        if (document) dispatch({ type: 'selected', documentUri: document.uri });
    }, [document, dispatch]);
    useEffect(
        /** Cancels analysis when the document or consent state changes. */
         ()=>{if(!enabled){attempted.current = false; cancel();}},[enabled,cancel]);
    /** Runs the connected analysis source and forwards its progress. */
    const start = useCallback( async () => {
        if (!document || current.current || !enabled) return;
        const runSource = source;
        if (!runSource) return;
        const controller = new AbortController();
        current.current = { controller, uri: document.uri };
        /** Checks whether a response still belongs to the current analysis run. */
        const active =  () => current.current?.controller === controller && !controller.signal.aborted;
        setMode('live');
        dispatch({ type: 'started', documentUri: document.uri });
        try {
            await runSource({ document, signal: controller.signal,
            /** Forwards service progress for the active document. */
            onProgress:  percent => {
                if (active() && Number.isFinite(percent)) dispatch({ type: 'progress', documentUri: document.uri, percent: Math.min(99, percent) });
            } });
            if (active()) dispatch({ type: 'progress', documentUri: document.uri, percent: 100 });
        } catch (error) {
            if (active()) dispatch({ type: 'failed', documentUri: document.uri, error: analysisErrorMessage(error) });
        } finally {
            if (current.current?.controller === controller) current.current = null;
        }
    }, [document, dispatch, source,enabled]);
    useEffect(() => {
        if (!document || !source || !enabled || attempted.current) return;
        // Strict Mode can clean up its first setup before an upload starts.
        const timer = setTimeout(() => {attempted.current = true; void start();}, 0);
        return () => clearTimeout(timer);
    }, [document, source, enabled, start]);
    return { mode, connected: !!source, start, cancel };
}
