const {test} = require('node:test');
const assert = require('node:assert/strict');
const React = require('react');
const renderer = require('react-test-renderer');
const load = require('./load-module.cjs');
global.IS_REACT_ACT_ENVIRONMENT = true;
const native = {View: 'view', Pressable: 'button', StyleSheet: {create: value => value}};
const dependencies = {'react-native': native, '../components/Text': {__esModule: true, default: 'text'}};
const results = load('analysis/DocumentResults.tsx', dependencies);
const {documentEntry} = load('upload/documentFlow.ts');

test('document flow requires CV then transcript before returning to results', () => {
    assert.equal(documentEntry('idle', 'idle'), 'upload');
    assert.equal(documentEntry('processing', 'idle'), 'upload');
    assert.equal(documentEntry('completed', 'idle'), 'uploadTranscript');
    assert.equal(documentEntry('completed', 'failed'), 'uploadTranscript');
    assert.equal(documentEntry('completed', 'completed'), 'analysis');
    assert.equal(documentEntry('failed', 'completed'), 'upload');
});
test('actual CV metrics and transcript marks are rendered, including zero marks', async () => {
    let root;
    await React.act(async () => {root = renderer.create(React.createElement(results.default, {
        atsScore: 74, formattingScore: 88,
        transcript: {averageMark: 42.5, summary: 'Read two subjects', subjects: [{subject: 'Maths', mark: 0}, {subject: 'Computing', mark: 85}]},
        onTranscript() {},
    }));});
    assert.deepEqual(root.root.findByProps({testID: 'cv-ats-score'}).children, ['74%']);
    assert.deepEqual(root.root.findByProps({testID: 'cv-formatting-score'}).children, ['88%']);
    assert.deepEqual(root.root.findByProps({testID: 'transcript-average-mark'}).children, ['42.5%']);
    assert.deepEqual(root.root.findByProps({testID: 'transcript-distinction-count'}).children, ['1']);
    assert.equal(root.root.findByProps({testID:'transcript-academic-summary'}).children.length,2);
    await React.act(async () => root.root.findAllByProps({accessibilityRole: 'button'}).find(node => node.props.accessibilityState).props.onPress());
    assert.ok(root.root.findAllByType('text').some(node => node.children.includes('0%')));
    await React.act(async () => root.unmount());
    assert.equal(results.percentage(null), '—');
    assert.equal(results.percentage(NaN), '—');
});
test('transcript report counts distinctions at 75 percent and above', async () => {
    let root;
    await React.act(async () => {root = renderer.create(React.createElement(results.default, {
        transcript: {averageMark:80.42, summary:'Transcript summary', subjects:[
            {subject:'Boundary', mark:75}, {subject:'High mark', mark:97},
            {subject:'Below threshold', mark:74.99}, {subject:'No mark', mark:null},
        ]}, onTranscript() {},
    }));});
    assert.deepEqual(root.root.findByProps({testID:'transcript-distinction-count'}).children,['2']);
    await React.act(async () => root.unmount());
});
test('progress ring uses an SVG rotation transform without origin props', async () => {
    const CircleProgress = load('upload/CircularProgress.tsx', {...dependencies,
        './UploadVector': {__esModule: true, default: 'vector'},
        'react-native-svg': {__esModule: true, default: 'svg', Circle: 'circle'},
    }).default;
    let root;
    await React.act(async () => {root = renderer.create(React.createElement(CircleProgress, {progress: 75}));});
    const circle = root.root.findAllByType('circle')[1];
    assert.equal(circle.props.transform, 'rotate(-90 47 47)');
    assert.equal(circle.props.origin, undefined);
    assert.equal(circle.props.rotation, undefined);
    assert.equal(circle.props.strokeDashoffset, 2*Math.PI*44*.25);
    await React.act(async () => root.update(React.createElement(CircleProgress, {progress: 50, gold: true})));
    assert.equal(root.root.findAllByType('circle')[1].props.strokeDashoffset, 2*Math.PI*44*.5);
    await React.act(async () => root.update(React.createElement(CircleProgress, {progress: 100, gold: true})));
    assert.equal(root.root.findAllByType('circle')[1].props.strokeDashoffset, 0);
    await React.act(async () => root.update(React.createElement(CircleProgress, {progress: 0})));
    assert.equal(root.root.findAllByType('circle').length, 1);
    await React.act(async () => root.unmount());
});


test('transcript summary shows the average and highlights without raw evidence', async()=>{
    let root;
    await React.act(async()=>{root=renderer.create(React.createElement(results.default,{
        transcript:{academicScore:78,averageMark:86.5,institution:'Example University',qualification:'BSc',summary:'Two modules',subjects:[],strengths:['Programming'],skillEvidence:[{skillName:'SQL',evidence:'Database Systems 91%'}]},onTranscript(){}
    }));});
    assert.deepEqual(root.root.findByProps({testID:'transcript-average-mark'}).children,['86.5%']);
    assert.ok(JSON.stringify(root.toJSON()).includes('Programming'));
    assert.ok(!JSON.stringify(root.toJSON()).includes('Database Systems 91%'));
    await React.act(async()=>root.unmount());
});
