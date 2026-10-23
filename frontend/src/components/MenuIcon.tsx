/** Draws the three-line menu icon. */
import Svg, { Path } from 'react-native-svg';

// Native equivalent of the Figma Simple Design System Menu, node 208:5375.
/** Draws the three-line menu icon. */
export default function MenuIcon({size=32,color='#a6a6a4'}:{size?:number;color?:string}) {
    return <Svg testID="navbar-menu-icon" width={size} height={size} viewBox="0 0 24 24" fill="none" stroke={color} strokeWidth={2} strokeLinecap="round" strokeLinejoin="round"><Path d="M3 6H21M3 12H21M3 18H21"/></Svg>;
}
