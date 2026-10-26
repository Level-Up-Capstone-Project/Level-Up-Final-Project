const {test} = require('node:test');
const assert = require('node:assert/strict');
const React = require('react');
const renderer = require('react-test-renderer');
const load = require('./load-module.cjs');

global.IS_REACT_ACT_ENVIRONMENT = true;

test('a previously missing application link no longer leaves an error after opening the link', async () => {
    const opened = [];
    const component = name => ({__esModule:true, default:name});
    const Job = load('screens/JobDetailScreen.tsx', {
        'react-native':{Image:'image',Linking:{openURL:async url=>opened.push(url)},Pressable:'button',ScrollView:'scroll',StyleSheet:{absoluteFill:{},create:value=>value},View:'view'},
        'react-native-safe-area-context':{useSafeAreaInsets:()=>({top:0,bottom:0})},
        'react-native-svg':{__esModule:true,default:'svg',Defs:'defs',LinearGradient:'gradient',Rect:'rect',Stop:'stop'},
        '../components/VignettePair':component('vignette'),
        '../components/ScreenHeader':{__esModule:true,default:'header',screenHeaderTop:()=>80},
        '../components/Text':component('text'), '../components/ChevronLeft':component('chevron'),
        '../opportunities/OpportunityIcon':component('icon'), '../onboarding/LocalVector':component('vector'),
        '../layout/useResponsiveStyles':{__esModule:true,default:value=>value},
    }).default;
    const selected = {id:'job',company:'Company',title:'Developer',area:'',region:'',workModel:'Remote',employmentType:'Full-time',match:null};
    const model = {width:408,opportunityModel:{selected,saved:[],toggleSaved(){}},goBack(){},go(){}};
    let root;
    await React.act(async()=>{root=renderer.create(React.createElement(Job,{model}));});
    await React.act(async()=>root.root.findByProps({accessibilityRole:'link'}).props.onPress());
    assert.match(JSON.stringify(root.toJSON()),/A company application link is not available yet/);
    model.opportunityModel.selected = {...selected,applicationUrl:'https://example.com/job'};
    await React.act(async()=>root.update(React.createElement(Job,{model})));
    await React.act(async()=>root.root.findByProps({accessibilityRole:'link'}).props.onPress());
    assert.deepEqual(opened,['https://example.com/job']);
    assert.doesNotMatch(JSON.stringify(root.toJSON()),/A company application link is not available yet/);
    await React.act(async()=>root.unmount());
});
