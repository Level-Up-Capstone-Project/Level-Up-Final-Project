const {test} = require('node:test');
const assert = require('node:assert/strict');
const React = require('react');
const renderer = require('react-test-renderer');
const load = require('./load-module.cjs');
const useAnalysisRunner = load('upload/useAnalysisRunner.ts').default;
global.IS_REACT_ACT_ENVIRONMENT = true;
const {act} = React;
const pause = () => new Promise(resolve => setTimeout(resolve, 25));
let latest;
function Harness(props) { latest = useAnalysisRunner(props.document, props.dispatch, props.source, props.enabled); return null; }
const component = props => React.createElement(React.StrictMode, null, React.createElement(Harness, props));
const first = {uri: 'blob:first', name: 'cv.pdf'};

test('automatic document analysis', async t => {
    await t.test('starts exactly once in Strict Mode and does not restart after completion', async () => {
        let calls = 0;
        const events = [];
        const props = {document: first, dispatch: event => events.push(event), source: async () => {calls++;}, enabled: true};
        let root;
        await act(async () => {root = renderer.create(component(props));});
        await act(pause);
        assert.equal(calls, 1);
        assert.ok(events.some(event => event.type === 'progress' && event.percent === 100));
        await act(async () => {root.update(component(props));});
        await act(pause);
        assert.equal(calls, 1);
        await act(async () => root.unmount());
    });
    await t.test('waits for consent then starts without another click', async () => {
        let calls = 0;
        const props = {document: first, dispatch() {}, source: async () => {calls++;}, enabled: false};
        let root;
        await act(async () => {root = renderer.create(component(props));});
        await act(pause);
        assert.equal(calls, 0);
        await act(async () => root.update(component({...props, enabled: true})));
        await act(pause);
        assert.equal(calls, 1);
        await act(async () => root.unmount());
    });
    await t.test('replacing the file cancels the old run and starts the new document', async () => {
        const requests = [];
        const props = {document: first, dispatch() {}, enabled: true, source: request => new Promise(resolve => {
            requests.push(request);
            request.signal.addEventListener('abort', resolve, {once: true});
        })};
        let root;
        await act(async () => {root = renderer.create(component(props));});
        await act(pause);
        await act(async () => root.update(component({...props, document: {uri: 'blob:second', name: 'next.pdf'}})));
        await act(pause);
        assert.equal(requests.length, 2);
        assert.equal(requests[0].signal.aborted, true);
        assert.equal(requests[1].document.uri, 'blob:second');
        await act(async () => root.unmount());
        assert.equal(requests[1].signal.aborted, true);
    });
    await t.test('failure does not loop uploads and manual retry remains available', async () => {
        let calls = 0;
        const events = [];
        const props = {document: first, dispatch: event => events.push(event), enabled: true, source: async () => {calls++; throw new Error('OCR unavailable');}};
        let root;
        await act(async () => {root = renderer.create(component(props));});
        await act(pause);
        await act(pause);
        assert.equal(calls, 1);
        assert.ok(events.some(event => event.type === 'failed' && event.error === 'OCR unavailable'));
        await act(async () => latest.start());
        assert.equal(calls, 2);
        await act(async () => root.unmount());
    });
    await t.test('explains a document ownership rejection and does not retry automatically', async () => {
        let calls = 0;
        const events = [];
        const props = {document: first, dispatch: event => events.push(event), enabled: true, source: async () => {
            calls++; throw Object.assign(new Error('Forbidden'), {status: 403});
        }};
        let root;
        await act(async () => {root = renderer.create(component(props));});
        await act(pause);
        assert.equal(calls, 1);
        assert.equal(events.find(event => event.type === 'failed')?.error,
            'Name mismatch: this document must show the same first name and surname as your Level Up profile. Update your profile to match the name on your ID, CV or institution record, then try again.');
        await act(async () => root.unmount());
    });
});
