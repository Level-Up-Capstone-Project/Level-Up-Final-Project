/** Provides the main styled action button for authentication forms. */
import { useEffect, useRef } from 'react';
import { Animated, Platform, Pressable, Text, View, useWindowDimensions } from 'react-native';
import { useAuthStyles } from '../styles';
import AuthIcon from './AuthIcon';
import Svg, { Defs, LinearGradient, Rect, Stop } from 'react-native-svg';
/** Provides the main styled action button for authentication forms. */
export default function PrimaryAction({ label, onPress, loading = false, marginTop, textColor }: {
    label: string;
    onPress: () => void;
    loading?: boolean;
    marginTop?: number;
    textColor?: string;
}) {
    const styles = useAuthStyles();
    const { width } = useWindowDimensions();
    const rotation = useRef(new Animated.Value(0)).current;
    useEffect(
        /** Updates the component animation and its cleanup. */
         () => {
        if (!loading)
            return;
        const animation = Animated.loop(Animated.timing(rotation, { toValue: 1, duration: 1000, useNativeDriver: Platform.OS !== 'web' }));
        animation.start();
        /** Runs the effect cleanup when its dependencies change or the component closes. */
        return  () => { animation.stop(); rotation.setValue(0); };
    }, [loading, rotation]);
    return <Pressable accessibilityRole="button" accessibilityState={{ disabled: loading, busy: loading }} disabled={loading} onPress={onPress} style={
        /** Returns the control styles for its current pressed state. */
         ({ pressed }) => [styles.primary, { marginTop: marginTop ?? 32 * Math.min(width, 480) / 408.393 }, pressed && { opacity: 0.85 }]}>
    <Svg pointerEvents="none" width="100%" height="100%" style={{ position: 'absolute', top: 0, left: 0 }}><Defs><LinearGradient id="authGold" x1="0%" y1="0%" x2="0%" y2="100%"><Stop offset={0} stopColor="#dfa94a"/><Stop offset={0.5} stopColor="#c98c30"/><Stop offset={1} stopColor="#a86e1c"/></LinearGradient><LinearGradient id="authShine" x1="0%" y1="0%" x2="0%" y2="100%"><Stop offset={0} stopColor="white" stopOpacity={0.333}/><Stop offset={0.5} stopColor="white" stopOpacity={0}/></LinearGradient></Defs><Rect width="100%" height="100%" rx={14} fill="url(#authGold)"/><Rect width="100%" height="100%" rx={14} fill="url(#authShine)"/></Svg>
    {loading && <Animated.View style={[styles.spinner, { transform: [{ rotate: rotation.interpolate({ inputRange: [0, 1], outputRange: ['0deg', '360deg'] }) }] }]}><AuthIcon name="loading"/></Animated.View>}
    <Text style={[styles.primaryText,textColor ? {color:textColor} : undefined]}>{loading ? 'Submitting...' : label}</Text>
  </Pressable>;
}
