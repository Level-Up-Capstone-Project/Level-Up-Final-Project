const {test} = require('node:test');
const assert = require('node:assert/strict');
const {earnedMedals} = require('./load-module.cjs')('achievements/eligibility.ts');
test('locked medals cannot open details, earned medals can', async () => {
    const React = require('react');
    const renderer = require('react-test-renderer');
    const load = require('./load-module.cjs');
    global.IS_REACT_ACT_ENVIRONMENT = true;
    const Screen = load('screens/AchievementsScreen.tsx', {
        'react-native': {Platform: {OS: 'web'}, BackHandler: {}, Image: 'image', Modal: 'modal', Pressable: 'button', ScrollView: 'scroll', View: 'view', StyleSheet: {create: value => value}},
        'react-native-safe-area-context': {useSafeAreaInsets: () => ({top: 0, bottom: 0})},
        '../components/ScreenHeader': {__esModule: true, default: 'header', screenHeaderTop: () => 0},
        '../components/Text': {__esModule: true, default: 'text'},
        '../components/ChevronLeft': {__esModule: true, default: 'chevron'},
        '../onboarding/LocalVector': {__esModule: true, default: 'vector'},
        '../achievements/data': load('achievements/data.ts'),
    }).default;
    const model = {width: 408, earnedBadgeIds: [], go() {}};
    let root;
    await React.act(async () => {root = renderer.create(React.createElement(Screen, {model}));});
    const badge = () => root.root.findAllByType('button').find(button => button.props.accessibilityLabel?.startsWith('First Ascent'));
    assert.equal(badge().props.disabled, true);
    assert.equal(badge().props.accessibilityState.disabled, true);
    await React.act(async () => badge().props.onPress());
    assert.equal(root.root.findByType('modal').props.visible, false);
    await React.act(async () => root.update(React.createElement(Screen, {model: {...model, earnedBadgeIds: ['first-ascent']}})));
    assert.equal(badge().props.disabled, false);
    await React.act(async () => badge().props.onPress());
    assert.equal(root.root.findByType('modal').props.visible, true);
    await React.act(async () => root.unmount());
});
test('medals remain locked without evidence', () => {
    assert.deepEqual(earnedMedals({}), []);
    assert.deepEqual(earnedMedals({readiness: null, verifiedSkills: 0}), []);
    assert.equal(earnedMedals({cvAnalysed: false}).includes('mission-ready'), false);
    assert.equal(earnedMedals({cvAnalysed: true}).includes('mission-ready'), true);
});
test('readiness medals unlock only at valid goal thresholds', () => {
    assert.equal(earnedMedals({readiness: 24.99}).includes('forward-base'), false);
    assert.equal(earnedMedals({readiness: 25}).includes('forward-base'), true);
    assert.equal(earnedMedals({readiness: 99.99}).includes('summit-command'), false);
    assert.equal(earnedMedals({readiness: 100}).includes('summit-command'), true);
    for (const readiness of [NaN, Infinity, 101, -1]) assert.deepEqual(earnedMedals({readiness}), []);
});
test('five mastered skills require verified evidence, and achievements are deduplicated', () => {
    assert.equal(earnedMedals({verifiedSkills: 4}).includes('the-big-five'), false);
    assert.equal(earnedMedals({verifiedSkills: 5}).includes('the-big-five'), true);
    const ids = earnedMedals({onboardingCompleted: true, profileCompleted: true, cvAnalysed: true, targetCareerSelected: true, opportunitiesExplored: true});
    assert.ok(ids.includes('elite-operator'));
    assert.equal(new Set(ids).size, ids.length);
    assert.equal(ids.includes('mission-accomplished'), false);
    assert.equal(ids.includes('iron-will'), false);
});
