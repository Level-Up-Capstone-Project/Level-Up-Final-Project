import type {BundledAsset} from '../../platform/assets';
/** Displays the contour background and vignette used by authentication screens. */
import VignettePair from '../../components/VignettePair';
import { View } from 'react-native';
import LocalVector from '../../onboarding/LocalVector';

// Shared background from Figma 34:394, 34:345, 34:368 and 78:907.
/** Displays the contour background and vignette used by authentication screens. */
export default function AuthBackground({ width, height, top }: { width:number; height:number; top:number }) {
    /** Displays bundled vector artwork at the calculated screen scale. */
    const vector =  (source:BundledAsset,w:number,h:number,targetHeight:number) => <View style={{width:w,height:h,transformOrigin:'top left',transform:[{scaleX:width/w},{scaleY:targetHeight/h}]}}><LocalVector source={source} width={w} height={h}/></View>;
    return <View testID="auth-dark-background" pointerEvents="none" style={{position:'absolute',top,left:0,width,height,backgroundColor:'#000',overflow:'hidden'}}>
        <VignettePair height={height}/>
        <View testID="auth-upper-contours" style={{position:'absolute',top:0,opacity:.6}}>{vector({uri:'/assets/onboarding/0a3e3.svg'},409.194,183.735,height*.2066)}</View>
        <View testID="auth-lower-contours" style={{position:'absolute',top:height*.656,opacity:.6}}>{vector({uri:'/assets/onboarding/b18a2.svg'},409.194,305.376,height*.344)}</View>
    </View>;
}
