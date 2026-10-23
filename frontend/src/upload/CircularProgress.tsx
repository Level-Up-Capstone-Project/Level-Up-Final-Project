/** Displays a circular percentage indicator for document analysis. */
import { View } from 'react-native';
import Text from '../components/Text';
import Svg, { Circle } from 'react-native-svg';
/** Displays a circular percentage indicator for document analysis. */
export default function CircularProgress({ progress, size = 94, gold = false, label = 'CV analysis' }: { progress: number; size?: number; gold?: boolean; label?: string }) {
    const value = Number.isFinite(progress) ? Math.round(Math.min(100, Math.max(0, progress))) : 0;
    const circumference = 2 * Math.PI * 44;
    return <View accessibilityRole="progressbar" accessibilityLabel={label} accessibilityValue={{ min: 0, max: 100, now: value, text: `${value}%` }} aria-valuemin={0} aria-valuemax={100} aria-valuenow={value} style={{ width: size, height: size, justifyContent: 'center', alignItems: 'center' }}>
        <Svg pointerEvents="none" width={size} height={size} viewBox="0 0 94 94" style={{ position: 'absolute' }}>
            <Circle cx={47} cy={47} r={44} fill="none" stroke="#d1d1d6" strokeWidth={6}/>
            {value > 0 && <Circle cx={47} cy={47} r={44} fill="none" stroke={gold ? '#d4af53' : '#087a0e'} strokeWidth={6} strokeLinecap="round" strokeDasharray={[circumference, circumference]} strokeDashoffset={circumference * (1 - value / 100)} transform="rotate(-90 47 47)"/>}
        </Svg>
        <Text style={{ fontSize: size * 20 / 94, fontWeight: '700', color: '#fff', letterSpacing: .4 }}>{value}%</Text>
    </View>;
}
