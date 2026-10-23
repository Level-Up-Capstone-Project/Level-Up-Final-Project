const {test} = require('node:test');
const assert = require('node:assert/strict');
const React = require('react');
const renderer = require('react-test-renderer');
const load = require('./load-module.cjs');
global.IS_REACT_ACT_ENVIRONMENT = true;

test('Missions tab hub offers Intel and Missions cards to their existing screens', async () => {
    const component = name => ({__esModule:true, default:name});
    const header=props=>React.createElement('screen-header',props);
    const Hub = load('screens/MissionHubScreen.tsx', {
        'react-native':{
            BackHandler:{addEventListener:()=>({remove(){}})},Image:'image',Platform:{OS:'web'},
            Pressable:'button',ScrollView:'scroll',StyleSheet:{absoluteFill:{},create:value=>value},View:'view',
        },
        'react-native-safe-area-context':{useSafeAreaInsets:()=>({top:0,bottom:0})},
        '../components/ScreenHeader':{__esModule:true,default:header,screenHeaderTop:()=>80},
        '../components/Text':component('text'),
        '../navigation/BottomNavigation':component('nav'),
        '../opportunities/OpportunityIcon':component('icon'),
        '../components/VignettePair':component('vignette'),
        '../layout/useResponsiveStyles':{__esModule:true,default:value=>value},
    }).default;
    const destinations=[];
    const backDestinations=[];
    const model={width:408,screen:'missionHub',go:screen=>destinations.push(screen),goBack:fallback=>backDestinations.push(fallback)};
    let root;
    await React.act(async()=>{root=renderer.create(React.createElement(Hub,{model}));});

    const intel=root.root.findByProps({testID:'missions-intel-card'});
    const missions=root.root.findByProps({testID:'missions-roadmap-card'});
    assert.equal(intel.props.accessibilityLabel.startsWith('INTEL.'),true);
    assert.equal(missions.props.accessibilityLabel.startsWith('MISSIONS.'),true);
    assert.equal(root.root.findByProps({testID:'mission-hub-landscape'}).props.source.uri,'/assets/mission-hub/landscape.png');
    assert.equal(root.root.findByProps({testID:'mission-hub-upper-contours'}).props.source.uri,'/assets/settings/upper-contours.svg');
    assert.equal(root.root.findByProps({testID:'mission-hub-lower-contours'}).props.source.uri,'/assets/settings/lower-contours.svg');
    const screenHeader=root.root.findByType('screen-header');
    assert.equal(screenHeader.props.backLabel,'Back to previous page');
    await React.act(async()=>screenHeader.props.onBack());
    assert.deepEqual(backDestinations,['home']);
    await React.act(async()=>{intel.props.onPress();missions.props.onPress();});
    assert.deepEqual(destinations,['analysis','learning']);
    await React.act(async()=>root.unmount());
});
