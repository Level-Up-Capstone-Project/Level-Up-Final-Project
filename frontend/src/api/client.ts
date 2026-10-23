/** Sends JSON requests to the configured backend and reports API failures. */
export class ApiError extends Error {
    /** Retains the HTTP status and response details for screen error handling. */
    constructor(public status: number, public details: unknown) {
        super(errorMessage(details, status));
        this.name = 'ApiError';
    }
}

function errorMessage(details: unknown, status: number): string {
    if (typeof details === 'string' && details.trim()) return details;
    if (typeof details === 'object' && details !== null) {
        const problem = details as Record<string, unknown>;
        if (typeof problem.message === 'string') return problem.message;
        if (problem.errors && typeof problem.errors === 'object') {
            const messages = Object.values(problem.errors).flat().filter(value => typeof value === 'string');
            if (messages.length) return messages.join(' ');
        }
        if (typeof problem.detail === 'string') return problem.detail;
        if (typeof problem.title === 'string') return problem.title;
    }
    return status === 401 ? 'Your session has expired. Please sign in again.' : `Request failed (${status}).`;
}

type JsonRequest = Omit<RequestInit, 'body'> & {body?: unknown; token?: string; onProgress?: (percent: number) => void};

/** Adds JSON and optional bearer headers without persisting credentials. */
export async function apiRequest<T>(path: string, {body, token, headers, onProgress, ...options}: JsonRequest = {}): Promise<T> {
    const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;
    if (!baseUrl) throw new Error('The backend URL is not configured.');
    const requestHeaders = new Headers(headers);
    requestHeaders.set('Accept', onProgress ? 'application/x-ndjson' : 'application/json');
    const multipart = typeof FormData !== 'undefined' && body instanceof FormData;
    if (multipart) requestHeaders.delete('Content-Type');
    else if (body !== undefined) requestHeaders.set('Content-Type', 'application/json');
    if (token) requestHeaders.set('Authorization', `Bearer ${token}`);
    const response = await fetch(`${baseUrl.replace(/\/$/, '')}/${path.replace(/^\//, '')}`, {
        ...options, cache: 'no-store', headers: requestHeaders,
        body: multipart ? body as FormData : body === undefined ? undefined : JSON.stringify(body),
    });
    if (response.ok && onProgress && response.headers.get('content-type')?.includes('application/x-ndjson')) {
        if (!response.body) throw new Error('The analysis progress connection is unavailable. Try again.');
        const reader = response.body.getReader(), decoder = new TextDecoder();
        let buffer = '', completed = false, result: T | undefined;
        const receive = (line: string) => {
            if (!line.trim()) return;
            const event = JSON.parse(line);
            if (event.type === 'error') throw new ApiError(event.status ?? 500, event.details);
            if (event.type === 'progress' && Number.isFinite(event.percent)) onProgress(Math.max(0, Math.min(99, event.percent)));
            if (event.type === 'result') {result = event.data as T; completed = true;}
        };
        try {
            while (!completed) {
                const {value, done} = await reader.read();
                buffer += decoder.decode(value, {stream: !done});
                let end: number;
                while ((end = buffer.indexOf('\n')) >= 0) {
                    receive(buffer.slice(0, end)); buffer = buffer.slice(end + 1);
                }
                if (done) {receive(buffer); break;}
            }
            if (!completed) throw new Error('The analysis connection closed before results were ready. Try again.');
            return result as T;
        } finally {await reader.cancel().catch(() => {}); reader.releaseLock();}
    }
    const text = await response.text();
    let data: unknown = text || undefined;
    if (text && response.headers.get('content-type')?.includes('json')) {
        try { data = JSON.parse(text); } catch { throw new ApiError(response.status, 'The server returned invalid JSON.'); }
    }
    if (!response.ok) throw new ApiError(response.status, data);
    return data as T;
}
