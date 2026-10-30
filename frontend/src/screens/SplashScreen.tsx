/** Displays the opening brand screen and introduction entry point. */
import { useEffect, useState } from 'react';
import { Image, Pressable, StyleSheet, useWindowDimensions, View } from 'react-native';
import type { IntroProps } from '../onboarding/IntroLayout';
/** Displays the opening brand screen and introduction entry point. */
export default function SplashScreen({ go }: IntroProps) {
    const { width, height } = useWindowDimensions();
    const [ready, setReady] = useState(false);
    const [canvas, setCanvas] = useState({ width, height });
    const canvasWidth = canvas.width;
    useEffect(() => {
        setCanvas({ width, height });
    }, [width, height]);
    useEffect(
        /** Synchronises component state with the effect dependencies. */
         () => {
        if (!ready) return;
        /** Opens the introduction after the splash-screen delay. */
        const timer = setTimeout( () => go('introAssessment'), 6000);
        /** Clears the pending timer when the effect ends. */
        return  () => clearTimeout(timer);
    }, [ready, go]);
    return <View style={styles.safe}>
        <Pressable accessibilityRole="button" accessibilityLabel="Level Up. Your next mission starts here. Continue to introduction" onPress={
            /** Opens the intro assessment screen. */
             () => go('introAssessment')} onLayout={
            /** Records the measured layout dimensions for this view. */
             ({ nativeEvent: { layout } }) => setCanvas({ width: layout.width, height: layout.height })} style={styles.canvas} testID="splash-screen">
            <Image testID="splash-landscape" source={{uri:'/assets/onboarding/a7668.png'}} onLoad={
                /** Handles the on load for this component. */
                 () => setReady(true)} onError={
                /** Handles the on error for this component. */
                 () => setReady(true)} resizeMode="stretch" style={{ position: 'absolute', width: '121.97%', height: '100.4%', left: '-21.62%', top: 0 }}/>
            <Image testID="splash-logo" source={{uri:'/assets/onboarding/2a63a.png'}} style={{ position: 'absolute', width: canvasWidth * 189 / 408.393, height: canvasWidth * 189 / 408.393, top: canvas.height * 73 / 885.372, left: canvasWidth * 114 / 408.393 }}/>
        </Pressable>
    </View>;
}
const styles = StyleSheet.create({
    safe: { flex: 1, backgroundColor: '#000' },
    // Use the device viewport rather than a fixed design-width canvas.
    canvas: { flex: 1, width: '100%', alignSelf: 'stretch', overflow: 'hidden' },
});
