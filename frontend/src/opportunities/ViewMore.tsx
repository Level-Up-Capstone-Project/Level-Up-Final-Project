/** Displays the opportunity view-more control. */
import { Pressable, StyleSheet } from 'react-native';
import Svg, { Circle } from 'react-native-svg';
import Text from '../components/Text';
import useResponsiveStyles from '../layout/useResponsiveStyles';

/** Displays the opportunity view-more control. */
export default function ViewMore({ onPress }: { onPress: () => void }) {
    const s = useResponsiveStyles(styles);
    return <Pressable accessibilityRole="button" accessibilityLabel="View more opportunities" onPress={onPress} style={s.button}>
        <Text style={s.label}>View more</Text>
        <Svg width={24} height={24} viewBox="0 0 24 24" fill="none" stroke="#fff" strokeWidth={2}>
            {[5, 12, 19].map(
                /** Renders a item entry from its data. */
                 cx => <Circle key={cx} cx={cx} cy={12} r={1}/>)}
        </Svg>
    </Pressable>;
}
const styles = StyleSheet.create({
    button: { minHeight: 44, alignSelf: 'center', flexDirection: 'row', alignItems: 'center', gap: 0, marginTop: -4, marginBottom: 4, paddingHorizontal: 12 },
    label: { color: '#fff', fontFamily: 'Inter', fontSize: 13, fontWeight: '700' },
});
