const {test} = require('node:test');
const assert = require('node:assert/strict');
const React = require('react');
const renderer = require('react-test-renderer');
const load = require('./load-module.cjs');

global.IS_REACT_ACT_ENVIRONMENT = true;

test('mission intel displays resource details and opens the real learning URL', async () => {
    const opened = [];
    const component = name => ({__esModule:true, default:name});
    const MissionIntel = load('screens/MissionIntelScreen.tsx', {
        'react-native':{
            BackHandler:{addEventListener:()=>({remove(){}})},
            Image:'image', Linking:{openURL:async url=>opened.push(url)}, Modal:'modal', Platform:{OS:'web'},
            Pressable:'button', ScrollView:'scroll', StyleSheet:{absoluteFill:{},create:value=>value}, View:'view',
        },
        'react-native-safe-area-context':{useSafeAreaInsets:()=>({top:0,bottom:0})},
        'react-native-svg':{__esModule:true,default:'svg',Defs:'defs',LinearGradient:'linearGradient',Rect:'rect',Stop:'stop'},
        '../components/VignettePair':component('vignette'),
        '../components/ScreenHeader':{__esModule:true,default:'header',screenHeaderTop:()=>80},
        '../components/Text':component('text'),
        '../components/ChevronLeft':component('chevron'),
        '../opportunities/OpportunityIcon':component('icon'),
        '../onboarding/LocalVector':component('vector'),
        '../layout/useResponsiveStyles':{__esModule:true,default:value=>value},
    }).default;

    const item = {
        roadmapItemResourceID:'link-id', isCompleted:false, completedAt:null,
        resource:{
            resourceID:'resource-id', resourceName:'PostgreSQL tutorial', resourceType:'Tutorial',
            url:'https://www.postgresql.org/docs/current/tutorial.html', provider:'PostgreSQL',
            difficultyLevel:'Beginner', costType:'Free', estDuration:'5 hours',
            description:'Learn PostgreSQL concepts and SQL.',
        },
    };
    const model = {
        width:408, topic:'Learn PostgreSQL', learningGroups:[{title:'Learn PostgreSQL',topics:['Learn PostgreSQL']}],
        currentTopic:{title:'Learn PostgreSQL',resources:[item]}, savedTopics:[], checked:[false], learningError:'',
        toggleSavedTopic(){},setChecked(){},completeTopic:async()=>true,go(){},goBack(){},
    };

    let root;
    await React.act(async()=>{root=renderer.create(React.createElement(MissionIntel,{model}));});
    const resource=root.root.findByProps({accessibilityLabel:'Open resource: PostgreSQL tutorial'});
    await React.act(async()=>resource.props.onPress());
    const open=root.root.findByProps({accessibilityLabel:'Open learning resource'});
    await React.act(async()=>open.props.onPress());

    assert.deepEqual(opened,['https://www.postgresql.org/docs/current/tutorial.html']);
    const rendered=JSON.stringify(root.toJSON());
    assert.match(rendered,/PostgreSQL/);
    assert.match(rendered,/Tutorial/);
    assert.match(rendered,/5 hours/);
    await React.act(async()=>root.unmount());
});

test('mission completion waits for persisted progress before opening the knowledge check screen', async () => {
    const navigated = [];
    let releaseCompletion;
    const completion = new Promise(resolve => {releaseCompletion = resolve;});
    const component = name => ({__esModule:true, default:name});
    const MissionIntel = load('screens/MissionIntelScreen.tsx', {
        'react-native':{
            BackHandler:{addEventListener:()=>({remove(){}})}, Image:'image', Linking:{openURL:async()=>{}}, Modal:'modal',
            Platform:{OS:'web'}, Pressable:'button', ScrollView:'scroll', StyleSheet:{absoluteFill:{},create:value=>value}, View:'view',
        },
        'react-native-safe-area-context':{useSafeAreaInsets:()=>({top:0,bottom:0})},
        'react-native-svg':{__esModule:true,default:'svg',Defs:'defs',LinearGradient:'linearGradient',Rect:'rect',Stop:'stop'},
        '../components/VignettePair':component('vignette'), '../components/ScreenHeader':{__esModule:true,default:'header',screenHeaderTop:()=>80},
        '../components/Text':component('text'), '../components/ChevronLeft':component('chevron'),
        '../opportunities/OpportunityIcon':component('icon'), '../onboarding/LocalVector':component('vector'),
        '../layout/useResponsiveStyles':{__esModule:true,default:value=>value},
    }).default;
    const resource={roadmapItemResourceID:'link',isCompleted:false,completedAt:null,resource:{resourceID:'resource',resourceName:'Course',resourceType:'course',url:'https://example.com',provider:'Example',difficultyLevel:'beginner',costType:'free',estDuration:'1h',description:'Learn'}};
    const model={width:408,topic:'Learn SQL',learningGroups:[{title:'Learn SQL',topics:['Learn SQL']}],currentTopic:{title:'Learn SQL',resources:[resource]},savedTopics:[],checked:[false],learningError:'',toggleSavedTopic(){},setChecked(){},completeTopic:()=>completion,go:screen=>navigated.push(screen),goBack(){}};

    let root, press;
    await React.act(async()=>{root=renderer.create(React.createElement(MissionIntel,{model}));});
    const complete=root.root.findByProps({accessibilityLabel:'Mark topic complete'});
    await React.act(async()=>{press=complete.props.onPress();await Promise.resolve();});
    assert.deepEqual(navigated,[]);
    assert.equal(root.root.findByProps({accessibilityLabel:'Mark topic complete'}).props.disabled,true);
    await React.act(async()=>{releaseCompletion(true);await press;});
    assert.deepEqual(navigated,['complete']);
    await React.act(async()=>root.unmount());
});
