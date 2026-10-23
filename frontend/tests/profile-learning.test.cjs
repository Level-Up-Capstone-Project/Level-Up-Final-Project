const {test} = require('node:test');
const assert = require('node:assert/strict');
const React = require('react');
const renderer = require('react-test-renderer');
const load = require('./load-module.cjs');

global.IS_REACT_ACT_ENVIRONMENT = true;

test('profile API loads and persists the backend profile contract', async () => {
    const calls = [];
    const profile = {profileID:'profile-id', userID:'user-id', targetCareerID:null, fName:'Ada', lName:'Lovelace'};
    const {loadProfile, saveProfile} = load('api/profile.ts', {'./client': {apiRequest: async (path, options) => {
        calls.push({path, options});
        return profile;
    }}});
    const signal = new AbortController().signal;

    assert.equal(await loadProfile('jwt', signal), profile);
    assert.equal(await saveProfile('jwt', {fName:'Ada', lName:'Lovelace', qualification:'BSc', fieldOfStudy:'Computing', targetCareerID:null}, signal), profile);
    assert.deepEqual(calls, [
        {path:'/api/profile', options:{token:'jwt', signal}},
        {path:'/api/profile', options:{method:'PUT', token:'jwt', body:{fName:'Ada', lName:'Lovelace', qualification:'BSc', fieldOfStudy:'Computing', targetCareerID:null}, signal}},
    ]);
});

test('learning progress loads, completes and saves through authenticated backend endpoints', async () => {
    let current, root, completed = false;
    const calls = [];
    class FakeApiError extends Error {constructor(status, message) {super(message); this.status = status;}}
    const apiRequest = async (path, options = {}) => {
        calls.push({path, method:options.method ?? 'GET', token:options.token});
        if (path === '/api/roadmaps/current') return {roadmapID:'roadmap', careerID:'career', title:'Route', progressPercentage:completed?100:0,
            items:[{roadmapItemID:'item', skillID:'skill', title:'Learn SQL', stepOrder:1, status:completed?'completed':'not_started'}]};
        if (path === '/api/roadmaps/items/item/resources') return [{roadmapItemResourceID:'link', isCompleted:completed, completedAt:completed?'2026-10-22T00:00:00Z':null,
            resource:{resourceID:'resource', resourceName:'SQL course', resourceType:'course', url:'https://example.com', provider:'Example', difficultyLevel:'beginner', estDuration:'1h', description:'Learn'}}];
        if (path === '/api/saved-resources' && !options.method) return [];
        if (path === '/api/roadmaps/items/item/resources/resource/complete') {completed = true; return {};}
        if (path === '/api/saved-resources/resource' && options.method === 'POST') return {};
        throw new Error(`Unexpected request: ${options.method ?? 'GET'} ${path}`);
    };
    const useLearningProgress = load('learning/useLearningProgress.ts', {'../api/client': {apiRequest, ApiError:FakeApiError}}).default;
    function Harness() {current = useLearningProgress('USER@example.com', 'jwt', 'career'); return null;}

    await React.act(async () => {root = renderer.create(React.createElement(Harness));});
    assert.equal(current.topic, 'Learn SQL');
    assert.deepEqual(current.checked, [false]);
    assert.deepEqual(current.resourcesForTopic('Learn SQL'), ['SQL course']);

    await React.act(async () => {
        current.setChecked([true]);
        while (!completed) await new Promise(resolve => setTimeout(resolve, 1));
        await new Promise(resolve => setTimeout(resolve, 1));
    });
    assert.deepEqual(current.checked, [true]);
    assert.deepEqual(current.completed, ['Learn SQL']);

    await React.act(async () => {await current.toggleSavedTopic();});
    assert.deepEqual(current.savedTopics, ['Learn SQL']);
    assert.ok(calls.some(call => call.path === '/api/roadmaps/items/item/resources/resource/complete' && call.method === 'POST' && call.token === 'jwt'));
    assert.ok(calls.some(call => call.path === '/api/saved-resources/resource' && call.method === 'POST' && call.token === 'jwt'));
    await React.act(async () => root.unmount());
});

test('missing roadmap is generated for the persisted target career', async () => {
    let current, root;
    class FakeApiError extends Error {constructor(status, message) {super(message); this.status = status;}}
    const apiRequest = async (path, options = {}) => {
        if (path === '/api/roadmaps/current') throw new FakeApiError(404, 'Missing');
        if (path === '/api/roadmaps/generate') {
            assert.equal(options.method, 'POST');
            assert.deepEqual(options.body, {careerID:'career-id'});
            return {roadmapID:'roadmap', careerID:'career-id', title:'Route', progressPercentage:100, items:[]};
        }
        if (path === '/api/saved-resources') return [];
        throw new Error(`Unexpected request: ${path}`);
    };
    const useLearningProgress = load('learning/useLearningProgress.ts', {'../api/client': {apiRequest, ApiError:FakeApiError}}).default;
    function Harness() {current = useLearningProgress('user@example.com', 'jwt', 'career-id'); return null;}

    await React.act(async () => {root = renderer.create(React.createElement(Harness));});
    assert.equal(current.learningError, '');
    assert.deepEqual(current.learningGroups, []);
    await React.act(async () => root.unmount());
});

test('no current roadmap loads career suggestions and generates a selected roadmap', async () => {
    let current, root, generated = false;
    const calls = [];
    class FakeApiError extends Error {constructor(status, message) {super(message); this.status = status;}}
    const suggestion = {careerID:'career-id', careerName:'Data Analyst', matchedSkills:[{skillName:'SQL', fromCv:true, fromTranscript:false}]};
    const apiRequest = async (path, options = {}) => {
        calls.push({path, method:options.method ?? 'GET', body:options.body});
        if (path === '/api/roadmaps/current') {
            if (!generated) throw new FakeApiError(404, 'Not Found');
            return {roadmapID:'roadmap', careerID:'career-id', careerName:'Data Analyst', title:'Data Analyst Route', progressPercentage:0, items:[]};
        }
        if (path === '/api/careers/suggestions') return {suggestions:[suggestion]};
        if (path === '/api/roadmaps/generate') {generated = true; return {roadmapID:'roadmap'};}
        if (path === '/api/saved-resources') return [];
        throw new Error(`Unexpected request: ${path}`);
    };
    const useLearningProgress = load('learning/useLearningProgress.ts', {'../api/client': {apiRequest, ApiError:FakeApiError}}).default;
    function Harness() {current = useLearningProgress('user@example.com', 'jwt'); return null;}

    await React.act(async () => {root = renderer.create(React.createElement(Harness));});
    assert.equal(current.learningError, '');
    assert.deepEqual(current.careerSuggestions, [suggestion]);
    assert.equal(current.roadmapCareerName, '');

    await React.act(async () => {await current.generateRoadmap('career-id');});
    assert.ok(calls.some(call => call.path === '/api/roadmaps/generate' && call.method === 'POST' && call.body.careerID === 'career-id'));
    assert.equal(current.learningError, '');
    assert.deepEqual(current.careerSuggestions, []);
    assert.equal(current.roadmapCareerName, 'Data Analyst');
    await React.act(async () => root.unmount());
});
