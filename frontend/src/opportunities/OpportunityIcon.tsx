/** Provides the icons used by opportunity screens and controls. */
import MenuIcon from '../components/MenuIcon';
import FigmaIcon from '../components/FigmaIcon';
import Svg, { Circle, Path, Rect } from 'react-native-svg';
// Native counterparts for unavailable external Figma icon components.
/** Provides the icons used by opportunity screens and controls. */
export default function OpportunityIcon({ name, size = 24, color = '#d4af53', filled = false }: { name: string; size?: number; color?: string; filled?: boolean }) {
    if(name==='menu')return <MenuIcon size={size} color={color}/>;
    if(name==='arrow'||name==='check'||name==='paperclip'||name==='plusSquare')return <FigmaIcon name={name} size={size} color={color}/>;
    return <Svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke={color} strokeWidth={1.8} strokeLinecap="round" strokeLinejoin="round">
        {name === 'search' ? <><Circle cx={10.5} cy={10.5} r={7.5}/><Path d="M16 16L22 22"/></> :
        name === 'filter' ? <Path d="M3 4H21L14 12V20L10 18V12Z"/> :
        name === 'bookmark' ? <Path d="M6 3H18V21L12 17L6 21Z" fill={filled ? color : 'none'}/> :
        name === 'bell' ? <><Path d="M5 17H19L17 14V9A5 5 0 0 0 7 9V14Z"/><Path d="M10 21H14"/></> :
        name === 'arrow' ? <Path d="M4 12H20M14 6L20 12L14 18"/> :
        name === 'profile' ? <><Circle cx={12} cy={7} r={4}/><Path d="M4 22V19A6 6 0 0 1 10 13H14A6 6 0 0 1 20 19V22"/></> :
        name === 'missions' ? <><Circle cx={12} cy={12} r={10}/><Circle cx={12} cy={12} r={6}/><Circle cx={12} cy={12} r={2}/></> :
        name === 'home' ? <><Path d="M3 10L12 2L21 10V22H3Z"/><Path d="M9 22V12H15V22"/></> :
        name === 'map' ? <Path d="M3 6L9 3L15 6L21 3V18L15 21L9 18L3 21ZM9 3V18M15 6V21"/> :
        name === 'learn' ? <><Path d="M12 5V22M2 3H8A4 4 0 0 1 12 7A4 4 0 0 1 16 3H22V20H16A4 4 0 0 0 12 22A4 4 0 0 0 8 20H2Z"/></> :
        <><Rect x={2} y={7} width={20} height={15} rx={2}/><Path d="M8 7V3H16V7M9 7V22M15 7V22"/></>}
    </Svg>;
}
