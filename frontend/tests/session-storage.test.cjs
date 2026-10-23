const {test} = require('node:test');
const assert = require('node:assert/strict');
const load = require('./load-module.cjs');

function memoryStorage() {
    const values = new Map();
    return {
        getItem: key => values.get(key) ?? null,
        setItem: (key, value) => values.set(key, String(value)),
        removeItem: key => values.delete(key),
    };
}

test('the signed-in route survives a refresh and legacy sessions are migrated', () => {
    const oldLocal = global.localStorage;
    const oldSession = global.sessionStorage;
    global.localStorage = memoryStorage();
    global.sessionStorage = memoryStorage();
    try {
        const session = load('navigation/sessionStorage.ts');
        session.saveSession({token: 'token', email: 'student@example.com', name: 'Student'}, 'analysis');
        assert.deepEqual(session.loadSession(), {token: 'token', email: 'student@example.com', name: 'Student', screen: 'analysis'});

        session.clearSession();
        global.sessionStorage.setItem('levelup-session', JSON.stringify({token: 'old-token', email: 'legacy@example.com', name: 'Legacy', screen: 'learning'}));
        assert.equal(session.loadSession().screen, 'learning');
        assert.equal(JSON.parse(global.localStorage.getItem('levelup-session')).token, 'old-token');
    } finally {
        global.localStorage = oldLocal;
        global.sessionStorage = oldSession;
    }
});
