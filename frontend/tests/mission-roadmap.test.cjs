const {test} = require('node:test');
const assert = require('node:assert/strict');
const React = require('react');
const renderer = require('react-test-renderer');
const load = require('./load-module.cjs');
global.IS_REACT_ACT_ENVIRONMENT = true;

async function createRoadmap(model) {
    const component = name => ({__esModule:true, default:name});
    const Roadmap = load('screens/MissionRoadmapScreen.tsx', {
        'react-native':{
            BackHandler:{addEventListener:()=>({remove(){}})},
            Image:'image',Platform:{OS:'web'},Pressable:'button',ScrollView:'scroll',
            StyleSheet:{absoluteFill:{},create:value=>value},View:'view',
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
    let root;
    await React.act(async()=>{root=renderer.create(React.createElement(Roadmap,{model}));});
    return root;
}

function baseModel(overrides={}) {
    return {
        roles:[],width:408,learningGroups:[{title:'SQL',topics:['SQL']}],
        resourcesForTopic:()=>['SQL joins'],completed:[],taskProgress:{},topic:'',
        setTopic(){},go(){},goBack(){},careerSuggestions:[],learningLoading:false,
        learningError:'',reloadLearning(){},...overrides,
    };
}

test('roadmap back button describes history-based navigation', async () => {
    const root=await createRoadmap(baseModel());
    const header=root.root.findByType('header');
    assert.equal(header.props.backLabel,'Back to previous page');
    await React.act(async()=>root.unmount());
});

test('Continue Learning opens the next topic', async () => {
    const navigated=[];
    const selected=[];
    const root=await createRoadmap(baseModel({
        setTopic:topic=>selected.push(topic),
        go:screen=>navigated.push(screen),
    }));

    const button=root.root.findByProps({testID:'roadmap-continue'});
    assert.equal(button.props.disabled,false);
    assert.equal(button.props.accessibilityLabel,'Continue Learning');
    await React.act(async()=>button.props.onPress());
    assert.deepEqual(selected,['SQL']);
    assert.deepEqual(navigated,['topic']);
    await React.act(async()=>root.unmount());
});

test('empty roadmap footer directs users to choose a career', async () => {
    const navigated=[];
    const root=await createRoadmap(baseModel({
        learningGroups:[],resourcesForTopic:()=>[],
        go:screen=>navigated.push(screen),
    }));

    const button=root.root.findByProps({testID:'roadmap-continue'});
    assert.equal(button.props.disabled,false);
    assert.equal(button.props.accessibilityLabel,'Choose a Career');
    await React.act(async()=>button.props.onPress());
    assert.deepEqual(navigated,['profileSettings']);
    await React.act(async()=>root.unmount());
});

test('roadmap footer retries after an error and is disabled only while loading', async () => {
    let retries=0;
    const errorRoot=await createRoadmap(baseModel({
        learningGroups:[],resourcesForTopic:()=>[],learningError:'Could not load roadmap.',
        reloadLearning:()=>{retries++;},
    }));
    const retry=errorRoot.root.findByProps({testID:'roadmap-continue'});
    assert.equal(retry.props.accessibilityLabel,'Retry Roadmap');
    await React.act(async()=>retry.props.onPress());
    assert.equal(retries,1);
    await React.act(async()=>errorRoot.unmount());

    const loadingRoot=await createRoadmap(baseModel({
        learningGroups:[],resourcesForTopic:()=>[],learningLoading:true,
    }));
    const loading=loadingRoot.root.findByProps({testID:'roadmap-continue'});
    assert.equal(loading.props.disabled,true);
    assert.equal(loading.props.accessibilityLabel,'Loading Roadmap…');
    await React.act(async()=>loadingRoot.unmount());
});
