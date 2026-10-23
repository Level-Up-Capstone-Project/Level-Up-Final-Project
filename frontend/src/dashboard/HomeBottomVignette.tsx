/** Draws the standalone lower fade for the home background. */
import Svg, { Defs, LinearGradient, Rect, Stop } from 'react-native-svg';

// Latest Figma 159:801, sized and positioned against the width-scaled canvas.
/** Draws the standalone lower fade for the home background. */
export default function HomeBottomVignette({ scale }: { scale:number }) {
    return <Svg testID="home-bottom-vignette" pointerEvents="none" width="99.9%" height={885.372*1.1103*scale} style={{position:'absolute',left:0,top:-885.372*.1107*scale}}>
        <Defs><LinearGradient id="home-bottom-vignette" x1="0" y1="0" x2="0" y2="1"><Stop offset={0} stopColor="#000" stopOpacity={0}/><Stop offset={.15285} stopColor="#000" stopOpacity={.3}/><Stop offset={.28872} stopColor="#000" stopOpacity={.8}/><Stop offset={.4076} stopColor="#000"/><Stop offset={1} stopColor="#000"/></LinearGradient></Defs>
        <Rect width="100%" height="100%" fill="url(#home-bottom-vignette)"/>
    </Svg>;
}
