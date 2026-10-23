/** Draws a check mark at the requested size and colour. */
import Svg, { Path } from 'react-native-svg';

// Native equivalent of Figma Simple Design System Check: 207:4886, 207:4889, 207:4892.
/** Draws a check mark at the requested size and colour. */
export default function CheckIcon({size=24,color='#d4af53'}:{size?:number;color?:string}) {
    return <Svg testID="figma-check-icon" width={size} height={size} viewBox="0 0 24 24" fill="none" stroke={color} strokeWidth={2} strokeLinecap="round" strokeLinejoin="round"><Path d="M20 6L9 17L4 12"/></Svg>;
}
