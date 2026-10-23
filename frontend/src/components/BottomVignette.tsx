/** Draws the standalone bottom-fade background treatment. */
import Svg, { Defs, LinearGradient, Rect, Stop } from 'react-native-svg';

// Figma 180:3641, relative to the 408.393px design canvas.
/** Draws the standalone bottom-fade background treatment. */
export default function BottomVignette({ scale = 1, testID = 'bottom-vignette', intel = false }: { scale?: number; testID?: string; intel?: boolean }) {
    return <Svg testID={testID} pointerEvents="none" width={421 * scale} height={(intel ? 901 : 1019) * scale} style={{ position:'absolute',left:-7 * scale,top:(intel ? -14 : -132) * scale }}>
        <Defs><LinearGradient id={testID} x1="0%" y1="0%" x2="0%" y2="100%">
            <Stop offset={0} stopColor="#000" stopOpacity={0}/>
            <Stop offset={0.15285} stopColor="#000" stopOpacity={0.285}/>
            <Stop offset={0.28872} stopColor="#000" stopOpacity={0.76}/>
            <Stop offset={0.4076} stopColor="#000" stopOpacity={0.95}/>
            <Stop offset={1} stopColor="#000" stopOpacity={0.95}/>
        </LinearGradient></Defs>
        <Rect width="100%" height="100%" fill={`url(#${testID})`}/>
    </Svg>;
}
