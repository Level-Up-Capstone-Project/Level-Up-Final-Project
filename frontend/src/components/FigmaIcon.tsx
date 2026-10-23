/** Selects and renders shared vector icons or bundled icon artwork. */
import CheckIcon from './CheckIcon';
import TargetIcon from './TargetIcon';
import { Image } from 'react-native';
import Svg, { Path, Rect } from 'react-native-svg';

export type FigmaIconName = 'check'|'arrow'|'chevronRight'|'paperclip'|'plusSquare'|'target'|'wave'|'accountSecurity'|'learningBordered';
// Native equivalents for the external Figma icon libraries; bitmap nodes retain their exact supplied artwork.
/** Selects and renders shared vector icons or bundled icon artwork. */
export default function FigmaIcon({name,size=24,color='#d4af53'}:{name:FigmaIconName;size?:number;color?:string}) {
    if(name==='check')return <CheckIcon size={size} color={color}/>;
    if(name==='target')return <TargetIcon size={size}/>;
    const source=name==='wave'?{uri:'/assets/home/wave.png'}:name==='accountSecurity'?{uri:'/assets/settings/account-security.png'}:name==='learningBordered'?{uri:'/assets/mira/learning.png'}:null;
    if(source)return <Image accessible={false} source={source} resizeMode="contain" style={{width:size,height:size}}/>;
    return <Svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke={color} strokeWidth={2} strokeLinecap="round" strokeLinejoin="round">
        {name==='arrow'?<Path d="M4 12H20M14 6L20 12L14 18"/>:name==='chevronRight'?<Path d="M9 5L16 12L9 19"/>:name==='plusSquare'?<><Rect x={3} y={3} width={18} height={18} rx={3}/><Path d="M8 12H16M12 8V16"/></>:<Path d="M8 13L15 6A3 3 0 0 1 19 10L10 19A5 5 0 0 1 3 12L13 2A6 6 0 0 1 21 10L11 20"/>}
    </Svg>;
}
