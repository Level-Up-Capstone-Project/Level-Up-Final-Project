const {test} = require('node:test');
const assert = require('node:assert/strict');
const React = require('react');
const renderer = require('react-test-renderer');
const load = require('./load-module.cjs');

global.IS_REACT_ACT_ENVIRONMENT = true;
const {act} = React;

class ApiError extends Error {
    constructor(status, details) {
        super(details?.message || `Request failed (${status}).`);
        this.status = status;
        this.details = details;
    }
}

let request;
let latest;
const useAuthModel = load('auth/useAuthModel.ts', {
    '../location/types': {locationSummary: () => ''},
    'react-native': {
        Alert: {}, Keyboard: {}, Platform: {OS: 'web'}, ScrollView: function ScrollView() {}, TextInput: function TextInput() {},
        BackHandler: {addEventListener: () => ({remove() {}})},
        useWindowDimensions: () => ({height: 800, width: 408}),
    },
    'react-native-safe-area-context': {useSafeAreaInsets: () => ({top: 0, right: 0, bottom: 0, left: 0})},
    './assets': {authAssets: {}},
    './account': {isValidContact: () => true},
    '../api/client': {apiRequest: (...args) => request(...args), ApiError},
    '../api/profile': {saveProfilePreferences: async () => {}},
    './google': {googleNeedsOnboarding: () => false, signInWithGoogle: async () => {throw new Error('not used');}},
}).default;

const account = {email: '', name: '', industry: '', phone: '', targetRoles: [], workAreas: [], location: ''};
const baseProps = {
    screen: 'signin', navigate() {}, onAuthenticated() {}, onPasswordReset() {}, account,
    updateAccount() {}, openLegal() {},
};

function Harness(props) {
    latest = useAuthModel(props);
    return null;
}

test('authentication recovery routing', async t => {
    await t.test('correct credentials for an unverified account open email verification', async () => {
        const navigations = [];
        const accountUpdates = [];
        request = async path => {
            assert.equal(path, '/api/auth/login');
            throw new ApiError(403, {code: 'email_verification_required', message: 'Email verification is required.'});
        };
        let root;
        await act(async () => {
            root = renderer.create(React.createElement(Harness, {
                ...baseProps,
                navigate: screen => navigations.push(screen),
                updateAccount: value => accountUpdates.push(value),
            }));
        });
        await act(async () => {
            latest.setContact('pending@example.com');
            latest.setPassword('CorrectPassword123!');
        });
        await act(async () => latest.signIn());

        assert.deepEqual(navigations, ['otp']);
        assert.equal(accountUpdates.at(-1).email, 'pending@example.com');
        assert.equal(accountUpdates.at(-1).token, undefined);
        await act(async () => root.unmount());
    });

    await t.test('wrong password-reset OTP stays on the OTP screen', async () => {
        const navigations = [];
        request = async path => {
            if (path === '/api/auth/password-reset/request') return {message: 'If the account exists, a code was sent.'};
            if (path === '/api/auth/password-reset/verify') {
                throw new ApiError(400, {message: 'The password reset code is invalid or expired.'});
            }
            throw new Error(`Unexpected request: ${path}`);
        };
        const props = {...baseProps, screen: 'forgot', navigate: screen => navigations.push(screen)};
        let root;
        await act(async () => {root = renderer.create(React.createElement(Harness, props));});
        await act(async () => latest.setRecoveryContact('person@example.com'));
        await act(async () => latest.startRecovery());
        await act(async () => root.update(React.createElement(Harness, {...props, screen: 'otp'})));
        await act(async () => latest.setOtp(['0', '0', '0', '0', '0', '0']));
        await act(async () => latest.verify());

        assert.deepEqual(navigations, ['otp']);
        assert.equal(latest.message, 'The password reset code is invalid or expired.');

        request = async path => {
            assert.equal(path, '/api/auth/password-reset/verify');
            return {message: 'The password reset code is valid.'};
        };
        await act(async () => latest.setOtp(['1', '2', '3', '4', '5', '6']));
        await act(async () => latest.verify());
        assert.deepEqual(navigations, ['otp', 'reset']);
        await act(async () => root.unmount());
    });
});
