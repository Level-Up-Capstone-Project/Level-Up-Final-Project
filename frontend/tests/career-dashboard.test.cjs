const {test} = require('node:test');
const assert = require('node:assert/strict');
const React = require('react');
const renderer = require('react-test-renderer');
const load = require('./load-module.cjs');
global.IS_REACT_ACT_ENVIRONMENT = true;

test('dashboard retains CV scores and loads tracked skills, then refreshes after analysis', async () => {
    let score = null, current, root, calls = 0;
    const useCareerAssessment = load('dashboard/useCareerAssessment.ts', {'../api/client': {apiRequest: async path => {
        if (path === '/api/profile/skills') return [{skillID: 'sql'}, {skillID: 'csharp'}];
        if (path === '/api/skill-gap') throw new Error('No target career');
        calls++;
        return {careerReadinessScore: score, cvQualityScore: 76, targetCareer: null};
    }}}).default;
    function Harness({revision}) {current = useCareerAssessment('user@example.com', 'jwt', revision); return null;}
    await React.act(async () => {root = renderer.create(React.createElement(Harness, {revision: 'before'}));});
    assert.equal(current.careerAssessment.cvQualityScore, 76);
    assert.equal(current.careerAssessment.readiness, null);
    assert.equal(current.careerAssessment.trackedSkillCount, 2);
    score = 63;
    await React.act(async () => root.update(React.createElement(Harness, {revision: 'after-analysis'})));
    assert.equal(current.careerAssessment.readiness, 63);
    assert.equal(calls, 2);
    await React.act(async () => root.unmount());
});

test('skills match loads target-career requirements and status-based scores', async () => {
    let current, root;
    const useCareerAssessment=load('dashboard/useCareerAssessment.ts',{'../api/client':{apiRequest:async path=>{
        if(path==='/api/profile/skills')return [{skillID:'sql-id',skillName:'SQL',status:'Have'}];
        if(path==='/api/skill-gap')return {career:{careerName:'Data Analyst'},skillMatchPercentage:50,skills:[
            {skillName:'SQL',status:'Have'},{skillName:'Power BI',status:'Partial'},{skillName:'Python',status:'Missing'},
        ]};
        return {careerReadinessScore:62,cvQualityScore:80,targetCareer:{careerName:'Data Analyst'}};
    }}}).default;
    function Harness(){current=useCareerAssessment('user','jwt','revision','Data Analyst',1);return null;}
    await React.act(async()=>{root=renderer.create(React.createElement(Harness));});
    assert.equal(current.careerAssessment.targetCareer,'Data Analyst');
    assert.equal(current.careerAssessment.skillMatch.role,1);
    assert.equal(current.careerAssessment.skillMatch.overall,50);
    assert.deepEqual(current.careerAssessment.skillMatch.skills.map(skill=>[skill.name,skill.score]),[['SQL',100],['Power BI',50],['Python',0]]);
    await React.act(async()=>root.unmount());
});

test('skills match shows existing user skills when no target career is selected', async () => {
    let current, root;
    const useCareerAssessment=load('dashboard/useCareerAssessment.ts',{'../api/client':{apiRequest:async path=>{
        if(path==='/api/profile/skills')return [{skillID:'sql-id',skillName:'SQL',status:'Have'},{skillID:'excel-id',skillName:'Excel',status:'Partial'}];
        if(path==='/api/skill-gap')throw new Error('No target career');
        return {careerReadinessScore:null,cvQualityScore:70,targetCareer:null};
    }}}).default;
    function Harness(){current=useCareerAssessment('user','jwt');return null;}
    await React.act(async()=>{root=renderer.create(React.createElement(Harness));});
    assert.equal(current.careerAssessment.skillMatch,undefined);
    assert.deepEqual(current.careerAssessment.skills.map(skill=>[skill.name,skill.score]),[['SQL',100],['Excel',50]]);
    assert.equal(current.careerAssessment.trackedSkillCount,2);
    await React.act(async()=>root.unmount());
});


test('home ring number and arc use the same available score', async () => {
    const component = name => ({__esModule:true, default:name});
    const Home = load('screens/HomeScreen.tsx', {
        'react-native': {View:'view',Text:'text',Image:'image',Pressable:'button',Modal:'modal',ScrollView:'scroll',StyleSheet:{create:s=>s},useWindowDimensions:()=>({height:900,width:408})},
        'react-native-safe-area-context': {useSafeAreaInsets:()=>({top:0,bottom:0})},
        'react-native-svg': {__esModule:true,default:'svg',Circle:'circle'},
        '../components/Text':component('text'), '../components/VignettePair':component('vignette'),
        '../dashboard/MissionLandscape':component('landscape'), '../onboarding/LocalVector':component('vector'),
        '../opportunities/OpportunityIcon':component('icon'), '../navigation/BottomNavigation':component('nav'),
        '../layout/useResponsiveStyles':{__esModule:true,default:s=>s},
    }).default;
    const model={width:408,screen:'home',account:{name:'Test'},careerAssessment:{readiness:null,cvQualityScore:69.8,trackedSkillCount:0,prioritySkills:[],skills:[]},careerAssessmentLoaded:false,completed:[],taskProgress:{},learningGroups:[],resourcesForTopic:()=>[],cvAnalysis:{status:'completed'},transcriptAnalysis:{status:'completed'},documentAnalysisEntry:'analysis',opportunityModel:{allOpportunities:[]},transcriptAssessment:{subjects:[{subject:'Page',mark:2}],averageMark:2},go(){}};
    let root;
    await React.act(async()=>{root=renderer.create(React.createElement(Home,{model}));});
    assert.match(JSON.stringify(root.toJSON()),/WELCOME BACK,/);
    assert.match(JSON.stringify(root.toJSON()),/"TEST"/);
    assert.equal(root.root.findByProps({testID:'home-landscape'}).props.source.uri,'/assets/home/landscape.png');
    assert.match(JSON.stringify(root.toJSON()),/Your command centre/);
    assert.match(JSON.stringify(root.toJSON()),/Next mission/);
    assert.deepEqual(root.root.findByProps({testID:'home-readiness'}).children,['—']);
    assert.equal(root.root.findAllByProps({testID:'readiness-arc'}).length,0);
    model.careerAssessmentLoaded=true;
    await React.act(async()=>root.update(React.createElement(Home,{model})));
    assert.deepEqual(root.root.findByProps({testID:'home-readiness'}).children,['70%']);
    assert.equal(root.root.findByProps({testID:'readiness-arc'}).props.strokeDashoffset,2*Math.PI*44*(1-.698));
    assert.ok(!JSON.stringify(root.toJSON()).includes('2% average'));
    assert.equal(root.root.findAllByProps({testID:'readiness-prerequisite'}).length,0);
    assert.equal(root.root.findAllByProps({testID:'home-transcript-summary'}).length,0);
    await React.act(async()=>root.unmount());
});

test('selected supported target career is saved before reloading readiness', async()=>{
    let saved=false,current,root;
    const useAssessment=load('dashboard/useCareerAssessment.ts',{'../api/client':{apiRequest:async(path,options)=>{
        if(path==='/api/profile/skills')return [];
        if(path==='/api/skill-gap')throw new Error('No target career');
        if(path==='/api/careers')return [{careerID:'career-id',careerName:'Software Developer'}];
        if(path==='/api/profile/target-career'){assert.equal(options.method,'PUT');assert.equal(options.body.careerID,'career-id');saved=true;return {};}
        return {careerReadinessScore:saved?35:null,cvQualityScore:70,targetCareer:saved?{careerName:'Software Developer'}:null};
    }}}).default;
    function Harness(){current=useAssessment('user','jwt','revision','Software Developer');return null;}
    await React.act(async()=>{root=renderer.create(React.createElement(Harness));});
    assert.equal(saved,true);
    assert.equal(current.careerAssessment.readiness,35);
    await React.act(async()=>root.unmount());
});


test('effect dependencies keep the same size when target role changes',async()=>{
    let root;
    const oldError=console.error, warnings=[];
    console.error=(...args)=>{if(String(args[0]).includes('changed size'))warnings.push(args);};
    const hook=load('dashboard/useCareerAssessment.ts',{'../api/client':{apiRequest:async path=>path==='/api/profile/skills'||path==='/api/careers'?[]:path==='/api/skill-gap'?Promise.reject(new Error('No target career')):{careerReadinessScore:null,cvQualityScore:70,targetCareer:null}}}).default;
    function Harness({role}){hook('user','jwt','revision',role);return null;}
    try{
        await React.act(async()=>{root=renderer.create(React.createElement(Harness,{role:''}));});
        await React.act(async()=>root.update(React.createElement(Harness,{role:'Developer'})));
        await React.act(async()=>root.update(React.createElement(Harness,{role:''})));
        assert.equal(warnings.length,0);
        await React.act(async()=>root.unmount());
    }finally{console.error=oldError;}
});
