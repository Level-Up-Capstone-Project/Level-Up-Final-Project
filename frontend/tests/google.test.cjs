const {test} = require('node:test');
const assert = require('node:assert/strict');
const load = require('./load-module.cjs');

test('Google popup checks its source and origin and handles unavailable credentials', async t => {
    const originalWindow = global.window;
    const originalBase = process.env.NEXT_PUBLIC_API_BASE_URL;
    process.env.NEXT_PUBLIC_API_BASE_URL = 'https://localhost:7286';
    try {
        const listeners = new Map();
        let closes = 0;
        const popup = {closed: false, location: {}, close() {closes++;}};
        global.window = {location: {origin: 'https://www.levelupnow.co.za'}, open: () => popup, addEventListener: (name, callback) => listeners.set(name, callback), removeEventListener: name => listeners.delete(name)};
        await t.test('reports missing configuration', async () => {
            const {signInWithGoogle} = load('auth/google.ts', {'../api/client': {apiRequest: async () => ({configured: false})}});
            await assert.rejects(signInWithGoogle(new AbortController().signal, 'signin'), /OAuth credentials/);
            assert.ok(closes > 0);
        });
        await t.test('only accepts the expected popup and backend', async () => {
            const {signInWithGoogle, googleNeedsOnboarding} = load('auth/google.ts', {'../api/client': {apiRequest: async () => ({configured: true})}});
            const pending = signInWithGoogle(new AbortController().signal, 'signup');
            await new Promise(resolve => setImmediate(resolve));
            assert.equal(popup.location.href, 'https://localhost:7286/api/auth/google/start?intent=signup&returnOrigin=https%3A%2F%2Fwww.levelupnow.co.za');
            const callback = listeners.get('message');
            const result = {login: {token: 'application-jwt', user: {email: 'user@example.com'}, profileCompleted: true}, isNewUser: false, name: 'Person'};
            callback({origin: 'https://attacker.example', source: popup, data: {type: 'levelup-google', result}});
            assert.equal(listeners.get('message'), callback);
            callback({origin: 'https://localhost:7286', source: {}, data: {type: 'levelup-google', result}});
            assert.equal(listeners.get('message'), callback);
            callback({origin: 'https://localhost:7286', source: popup, data: {type: 'levelup-google', result}});
            assert.deepEqual(await pending, result);
            assert.equal(listeners.has('message'), false);
            assert.equal(googleNeedsOnboarding(result), false);
            assert.equal(googleNeedsOnboarding({...result, isNewUser: true}), true);
            assert.equal(googleNeedsOnboarding({...result, login: {...result.login, profileCompleted: false}}), true);
        });
    } finally {
        global.window = originalWindow;
        if (originalBase === undefined) delete process.env.NEXT_PUBLIC_API_BASE_URL;
        else process.env.NEXT_PUBLIC_API_BASE_URL = originalBase;
    }
});
