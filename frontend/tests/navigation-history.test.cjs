const {test}=require('node:test');
const assert=require('node:assert/strict');
const load=require('./load-module.cjs');

const createNavigationHistory=load('navigation/navigationHistory.ts',{'./types':{}}).default;

test('back navigation returns to the route visited before the current route',()=>{
    const history=createNavigationHistory();
    history.push('home','settings');
    history.push('settings','profileSettings');

    assert.equal(history.back('profileSettings','home'),'settings');
    assert.equal(history.back('settings','home'),'home');
});

test('repeated routes do not create a misleading back destination',()=>{
    const history=createNavigationHistory();
    history.push('home','home');

    assert.equal(history.back('home','home'),'home');
});

test('a navigation reset clears routes from the signed-out session',()=>{
    const history=createNavigationHistory();
    history.push('home','settings');
    history.clear();

    assert.equal(history.back('signin','signin'),'signin');
});
