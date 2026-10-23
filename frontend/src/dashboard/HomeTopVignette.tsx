/** Draws the standalone upper fade for the home background. */
import Svg, { Defs, LinearGradient, Rect, Stop } from 'react-native-svg';

// Figma 159:802 occupies the top 16.6% of the 885.372px design canvas.
/** Draws the standalone upper fade for the home background. */
export default function HomeTopVignette({ scale }: { scale: number }) {
    return <Svg testID="home-top-vignette" pointerEvents="none" width="99.9%" height={885.372 * .166 * scale} style={{position:'absolute',left:0,top:0}}>
        <Defs><LinearGradient id="home-top-vignette" x1="0%" y1="0%" x2="0%" y2="100%">
            <Stop offset={0} stopColor="#000" stopOpacity={0.72}/>
            <Stop offset={0.5} stopColor="#000" stopOpacity={0.38}/>
            <Stop offset={1} stopColor="#000" stopOpacity={0}/>
        </LinearGradient></Defs>
        <Rect width="100%" height="100%" fill="url(#home-top-vignette)"/>
    </Svg>;
}
