/** Draws the two layered gradients used by vignette backgrounds. */
import {useId} from 'react';
import Svg,{Defs,LinearGradient,Rect,Stop} from 'react-native-svg';
// Figma 103:1625 + 103:1626 on their 411 x 885 reference canvas.
/** Draws the two layered gradients used by vignette backgrounds. */
export default function VignettePair({height=885.372,testID='vignette-pair'}:{height?:number;testID?:string}) {
 const id=useId().replace(/[^a-zA-Z0-9]/g,'');
 const lower=id+'lower',front=id+'front';
 return <Svg testID={testID} pointerEvents="none" width="100%" height={height} style={{position:'absolute',left:0,top:0}}>
 <Defs>
 <LinearGradient id={lower} x1="0" y1="0" x2="0" y2="1"><Stop offset={0} stopColor="#000" stopOpacity={0}/><Stop offset={.15285} stopColor="#000" stopOpacity={.3}/><Stop offset={.28872} stopColor="#000" stopOpacity={.8}/><Stop offset={.4076} stopColor="#000"/><Stop offset={1} stopColor="#000"/></LinearGradient>
 <LinearGradient id={front} x1="0" y1="0" x2="0" y2="1"><Stop offset={0} stopColor="#000" stopOpacity={0}/><Stop offset={.11538} stopColor="#000" stopOpacity={.3}/><Stop offset={.19712} stopColor="#000" stopOpacity={.8}/><Stop offset={.4076} stopColor="#000"/><Stop offset={1} stopColor="#000"/></LinearGradient>
 </Defs>
 <Rect testID="vignette-103-1625" x={0} y={height*407.8287048339844/885} width="100%" height={height*477.1712951660156/885} fill={`url(#${lower})`} opacity={.8}/>
 <Rect testID="vignette-103-1626" x={0} y={height*28/885} width="100%" height={height*857/885} fill={`url(#${front})`} opacity={.85}/>
 </Svg>;
}
