/** Lists bundled authentication artwork and its dimensions. */
// Original Figma vector exports. Dimensions are the SVG root dimensions.
export const authAssets = {
    google: { source: {uri:'/assets/auth/ca016.svg'}, width: 23.52, height: 24 },
    facebook: { source: {uri:'/assets/auth/c420c.svg'}, width: 24, height: 24 },
    eye: { source: {uri:'/assets/auth/7fd94.svg'}, width: 24, height: 24 },
    signupEye: { source: {uri:'/assets/auth/4139f.svg'}, width: 24, height: 24 },
    back: { source: {uri:'/assets/auth/000f3.svg'}, width: 24, height: 24 },
    resetBack: { source: {uri:'/assets/auth/d73c9.svg'}, width: 24, height: 24 },
    loading: { source: {uri:'/assets/auth/f232e.svg'}, width: 21, height: 21.75 },
    divider: { source: {uri:'/assets/auth/8aca4.svg'}, width: 153, height: 1 },
    signupDivider: { source: {uri:'/assets/auth/f83ef.svg'}, width: 153, height: 1 },
    time: { source: {uri:'/assets/auth/a072b.svg'}, width: 54, height: 18 },
    alarm: { source: {uri:'/assets/auth/fec73.svg'}, width: 18, height: 18 },
    wifi: { source: {uri:'/assets/auth/77942.svg'}, width: 18, height: 18 },
    bluetooth: { source: {uri:'/assets/auth/e4781.svg'}, width: 18, height: 18 },
    signal: { source: {uri:'/assets/auth/048e3.svg'}, width: 18, height: 18 },
    signinSignal: { source: {uri:'/assets/auth/bb8dd.svg'}, width: 18, height: 18 },
    signupSignal: { source: {uri:'/assets/auth/738ac.svg'}, width: 18, height: 18.7606 },
    battery: { source: {uri:'/assets/auth/8c57b.svg'}, width: 18, height: 18 },
    signinBattery: { source: {uri:'/assets/auth/824e5.svg'}, width: 18, height: 18 },
    signupBattery: { source: {uri:'/assets/auth/cb67a.svg'}, width: 18, height: 18.7606 },
} as const;
export type AuthAssetName = keyof typeof authAssets;
