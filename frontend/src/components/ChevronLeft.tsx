/** Draws the left chevron used by back controls. */
import Svg, { Path } from 'react-native-svg';

// Native equivalent of the Figma Simple Design System ChevronLeft component.
/** Draws the left chevron used by back controls. */
export default function ChevronLeft({ color = '#ffbe63', size = 40 }: { color?: string; size?: number }) {
    return <Svg width={size} height={size} viewBox="0 0 48 48"><Path d="M29 12L17 24L29 36" fill="none" stroke={color} strokeWidth={3.5} strokeLinecap="round" strokeLinejoin="round"/></Svg>;
}
