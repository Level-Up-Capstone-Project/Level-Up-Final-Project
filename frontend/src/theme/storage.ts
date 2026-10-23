/** Provides browser storage for a theme preference. */
/** Reads a saved theme preference. */
export function loadDarkTheme(): boolean {
    try { return localStorage.getItem('career-path-dark-theme') !== 'false'; } catch { return true; }
}
/** Writes a theme preference to storage. */
export function saveDarkTheme(dark: boolean) {
    try { localStorage.setItem('career-path-dark-theme', String(dark)); } catch { /* Session theme remains usable without storage. */ }
}
