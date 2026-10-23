/** Keeps the signed-in user and current screen across refreshes until sign-out or token expiry. */
import type {Account} from '../auth/account';
import type {Screen} from './types';

const key = 'levelup-session';
// Screens that depend on a transient selection reopen their parent list instead.
const restorable: Partial<Record<Screen, Screen>> = {
    home: 'home', missionHub: 'missionHub', upload: 'upload', uploadTranscript: 'uploadTranscript', mira: 'mira',
    cvReport: 'cvReport', analysis: 'analysis', skills: 'skills', jobs: 'jobs', job: 'jobs', learning: 'learning',
    topic: 'learning', complete: 'complete', achievements: 'achievements', profileSettings: 'profileSettings', settings: 'settings',
};

export type StoredSession = {token: string; email: string; name: string; screen: Screen};

function parseSession(raw: string | null): StoredSession | null {
    try {
        const value = JSON.parse(raw || 'null');
        const screen = value && restorable[value.screen as Screen];
        return value && typeof value.token === 'string' && value.token && typeof value.email === 'string' && screen
            ? {token: value.token, email: value.email, name: String(value.name ?? ''), screen} : null;
    } catch { return null; }
}

export function loadSession(): StoredSession | null {
    try {
        const saved = parseSession(localStorage.getItem(key));
        if (saved) return saved;
        // Migrate sessions created before refresh persistence was introduced.
        const legacy = parseSession(sessionStorage.getItem(key));
        if (legacy) localStorage.setItem(key, JSON.stringify(legacy));
        return legacy;
    } catch { return null; }
}

export function saveSession(account: Account, screen: Screen) {
    try {
        if (account.token && restorable[screen])
            localStorage.setItem(key, JSON.stringify({token: account.token, email: account.email, name: account.name, screen}));
        else if (!account.token) clearSession();
    } catch { /* Navigation still works without storage. */ }
}

export function clearSession() {
    try { localStorage.removeItem(key); } catch { /* Nothing to clear. */ }
    try { sessionStorage.removeItem(key); } catch { /* Nothing to clear. */ }
}
