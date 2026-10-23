import {apiRequest} from '../api/client';

export type GoogleResult = {login: {token: string; user: {email: string}; profileCompleted: boolean}; isNewUser: boolean; name: string};
export type GoogleIntent = 'signin' | 'signup';

/** Uses trusted backend account state instead of the button that started OAuth. */
export function googleNeedsOnboarding(result: GoogleResult): boolean {
    return result.isNewUser || result.login.profileCompleted !== true;
}

/** Opens Google's server OAuth flow and accepts results only from our exact backend and popup. */
export async function signInWithGoogle(signal: AbortSignal, intent: GoogleIntent): Promise<GoogleResult> {
    const base = process.env.NEXT_PUBLIC_API_BASE_URL;
    if (!base) throw new Error('The backend URL is not configured.');
    const origin = new URL(base).origin;
    const popup = window.open('about:blank', 'levelup-google', 'popup,width=520,height=680');
    if (!popup) throw new Error('Allow popups for this site, then try Google sign-in again.');
    try {
        const config = await apiRequest<{configured: boolean}>('/api/auth/google/config', {signal});
        if (!config.configured) throw new Error('Google sign-in needs OAuth credentials. Configure the backend Google client ID and secret.');
        return await new Promise<GoogleResult>((resolve, reject) => {
            const cleanup = () => {
                window.removeEventListener('message', onMessage);
                signal.removeEventListener('abort', abort);
                clearInterval(poll); clearTimeout(timer);
                popup.close();
            };
            const fail = (error: Error) => {cleanup(); reject(error);};
            const abort = () => fail(new Error('Google sign-in was cancelled.'));
            const onMessage = (event: MessageEvent) => {
                if (event.origin !== origin || event.source !== popup || event.data?.type !== 'levelup-google') return;
                if (event.data.error) { fail(new Error(String(event.data.error))); return; }
                const result = event.data.result as GoogleResult;
                if (!result?.login?.token || !result.login.user?.email) {fail(new Error('Google sign-in returned an invalid result.')); return;}
                cleanup(); resolve(result);
            };
            const poll = setInterval(() => {if (popup.closed) fail(new Error('Google sign-in was closed. Please retry.'));}, 500);
            const timer = setTimeout(() => fail(new Error('Google sign-in timed out. Please retry.')), 180000);
            window.addEventListener('message', onMessage);
            signal.addEventListener('abort', abort, {once: true});
            if (signal.aborted) {abort(); return;}
            popup.location.href = `${base.replace(/\/$/, '')}/api/auth/google/start?intent=${intent}&returnOrigin=${encodeURIComponent(window.location.origin)}`;
        });
    } finally { popup.close(); }
}
