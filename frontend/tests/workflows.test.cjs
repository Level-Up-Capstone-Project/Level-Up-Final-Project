const {test} = require('node:test');
const assert = require('node:assert/strict');
const React = require('react');
const renderer = require('react-test-renderer');
const load = require('./load-module.cjs');
global.IS_REACT_ACT_ENVIRONMENT = true;

test('MIRA sends authenticated chat requests to the backend endpoint', async () => {
    let current, root;
    const calls = [];
    const useMiraChat = load('chat/useMiraChat.ts', {'../api/client': {apiRequest: async (path, options) => {
        calls.push({path, options});
        return {message:'Try practising SQL joins.'};
    }}}).default;
    function Harness() {current = useMiraChat('jwt'); return null;}

    await React.act(async () => {root = renderer.create(React.createElement(Harness));});
    await React.act(async () => {await current.send('How can I practise SQL?');});
    assert.deepEqual(current.messages.map(message=>[message.role,message.text]), [
        ['user','How can I practise SQL?'], ['assistant','Try practising SQL joins.'],
    ]);
    assert.equal(calls[0].path, '/api/mira');
    assert.equal(calls[0].options.token, 'jwt');
    assert.deepEqual(calls[0].options.body, {message:'How can I practise SQL?'});
    await React.act(async () => root.unmount());
});

test('Knowledge Check generates questions for the selected roadmap skill and submits answers', async () => {
    const calls = [];
    const shares = [];
    const assessment = {skillAssessmentID:'attempt-id', attemptNumber:1, questions:[
        {questionID:'q1',questionText:'First?',choices:['A','B','C'],questionOrder:1},
        {questionID:'q2',questionText:'Second?',choices:['A','B','C'],questionOrder:2},
        {questionID:'q3',questionText:'Third?',choices:['A','B','C'],questionOrder:3},
    ]};
    const apiRequest = async (path,options) => {
        calls.push({path,options});
        return path === '/api/assessments' ? assessment : {
            scorePercentage:66.67,correctAnswers:2,totalQuestions:3,perfectScore:false,
            answers:[
                {questionID:'q1',questionText:'First?',userAnswer:'A',correctAnswer:'A',isCorrect:true},
                {questionID:'q2',questionText:'Second?',userAnswer:'A',correctAnswer:'A',isCorrect:true},
                {questionID:'q3',questionText:'Third?',userAnswer:'A',correctAnswer:'B',isCorrect:false},
            ],
            skill:{skillName:'SQL',status:'Have',isKnowledgeVerified:true},
        };
    };
    const text = ({children, ...props}) => React.createElement('text',props,children);
    const modal = ({visible,children}) => visible ? React.createElement('modal',null,children) : null;
    const svg = ({children,...props}) => React.createElement('svg',props,children);
    const KnowledgeCheckAction = load('learning/KnowledgeCheckAction.tsx', {
        'react-native':{Image:'image',Modal:modal,Platform:{OS:'ios'},Pressable:'button',ScrollView:'scroll',Share:{share:async value=>shares.push(value)},StyleSheet:{absoluteFillObject:{},create:value=>value},View:'view'},
        'react-native-svg':{__esModule:true,default:svg,Circle:'circle'},
        '../components/Text':{__esModule:true,default:text},
        '../api/client':{apiRequest,ApiError:class extends Error {}},
    }).default;
    let root, dashboardCalls=0;
    await React.act(async () => {root=renderer.create(React.createElement(KnowledgeCheckAction,{
        token:'jwt',item:{roadmapItemID:'roadmap-item',skillID:'skill-id',title:'Learn SQL'},
        displayName:'Alex Morgan',onDashboard:()=>dashboardCalls++,
    }));});
    await React.act(async () => {root.root.findByProps({testID:'knowledge-check-start'}).props.onPress();});
    assert.equal(calls.length,0);
    assert.match(JSON.stringify(root.toJSON()),/TEST YOUR/);
    assert.match(JSON.stringify(root.toJSON()),/Skills you’ll explore/);
    assert.match(JSON.stringify(root.toJSON()),/Start Assessment/);
    const introBackground=root.root.findByProps({testID:'knowledge-check-intro-background'});
    assert.equal(introBackground.props.source.uri,'/assets/home/landscape.png');
    assert.equal(introBackground.props.style.position,'absolute');
    assert.equal(introBackground.props.style.height,'100%');
    await React.act(async () => {await root.root.findByProps({testID:'knowledge-check-begin'}).props.onPress();});
    assert.equal(calls[0].path,'/api/assessments');
    assert.deepEqual(calls[0].options.body,{skillID:'skill-id',roadmapItemID:'roadmap-item'});
    assert.equal(calls[0].options.token,'jwt');

    const choices=root.root.findAllByProps({accessibilityRole:'radio'});
    await React.act(async () => {for (const choice of [choices[0],choices[3],choices[6]]) choice.props.onPress();});
    await React.act(async () => {await root.root.findByProps({testID:'knowledge-check-submit'}).props.onPress();});
    assert.equal(calls[1].path,'/api/assessments/attempt-id/submit');
    assert.deepEqual(calls[1].options.body.responses,[
        {questionID:'q1',userAnswer:'A'}, {questionID:'q2',userAnswer:'A'}, {questionID:'q3',userAnswer:'A'},
    ]);
    assert.match(JSON.stringify(root.toJSON()),/67%/);
    assert.match(JSON.stringify(root.toJSON()),/Great Progress!/);
    assert.match(JSON.stringify(root.toJSON()),/Alex Morgan/);
    assert.match(JSON.stringify(root.toJSON()),/XP earned/);
    assert.match(JSON.stringify(root.toJSON()),/—/);
    await React.act(async () => {await root.root.findByProps({testID:'assessment-share'}).props.onPress();});
    assert.deepEqual(shares,[{title:'Assessment results',message:'I scored 2/3 (67%) on my SQL knowledge check.'}]);
    assert.equal(root.root.findByProps({testID:'assessment-review-answers'}).props.style.height,52);
    assert.equal(root.root.findByProps({testID:'knowledge-check-dashboard'}).props.style.height,52);
    await React.act(async () => {root.root.findByProps({testID:'assessment-open-retry'}).props.onPress();});
    assert.match(JSON.stringify(root.toJSON()),/Every attempt helps you grow/);
    assert.ok(JSON.stringify(root.toJSON()).includes('"Question ","3"'));
    assert.ok(JSON.stringify(root.toJSON()).includes('"Attempt ","1"'));
    await React.act(async () => {root.root.findByProps({testID:'assessment-review-explanation'}).props.onPress();});
    assert.match(JSON.stringify(root.toJSON()),/Correct answer/);
    await React.act(async () => {root.root.findByProps({testID:'assessment-back-to-results'}).props.onPress();});
    await React.act(async () => {root.root.findByProps({testID:'assessment-open-retry'}).props.onPress();});
    await React.act(async () => {await root.root.findByProps({testID:'assessment-try-again'}).props.onPress();});
    assert.equal(calls.filter(call=>call.path==='/api/assessments').length,2);
    const retryChoices=root.root.findAllByProps({accessibilityRole:'radio'});
    await React.act(async () => {for (const choice of [retryChoices[0],retryChoices[3],retryChoices[6]]) choice.props.onPress();});
    await React.act(async () => {await root.root.findByProps({testID:'knowledge-check-submit'}).props.onPress();});
    await React.act(async () => {root.root.findByProps({testID:'knowledge-check-dashboard'}).props.onPress();});
    assert.equal(dashboardCalls,1);
    await React.act(async () => root.unmount());
});
