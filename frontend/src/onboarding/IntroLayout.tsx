import type {BundledAsset} from '../platform/assets';
/** Arranges introductory artwork, headings, descriptions and slide navigation. */
import VignettePair from '../components/VignettePair';
import { useEffect, useMemo } from 'react';
import { Platform, BackHandler, Image, PanResponder, Pressable, ScrollView, StyleSheet, useWindowDimensions, View, type ImageSourcePropType } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import Svg, { Defs, LinearGradient, Rect, Stop } from 'react-native-svg';
import Text from '../components/Text';
import LocalVector from './LocalVector';
import ChevronRight from '../components/ChevronRight';
import type { Screen } from '../navigation/types';

export type IntroProps = { go: (screen: Screen) => void };
const pages: Screen[] = ['introAssessment', 'introGap', 'introTraining', 'introOpportunities'];
/** Arranges introductory artwork, headings, descriptions and slide navigation. */
export default function IntroLayout({ go, step, image, title, description, copyLeft, titleOffset = 0 }: IntroProps & {
    step: number; image: ImageSourcePropType; title: string; description: string; copyLeft: number; titleOffset?: number;
}) {
    const { width, height } = useWindowDimensions();
    const insets = useSafeAreaInsets();
    const canvasWidth = Math.min(width, 480);
    const scale = canvasWidth / 408.393;
    const canvasHeight = Math.max(height, 885.372 * scale, insets.top + insets.bottom + 640 * scale);
    /** Moves to the next introductory screen or sign-in. */
    const next =  () => go(step === 3 ? 'signin' : pages[step + 1]);
    /** Moves to the preceding introductory screen. */
    const previous =  () => go(step === 0 ? 'splash' : pages[step - 1]);
    useEffect(
        /** Registers the hardware back action and its cleanup. */
         () => {
        /** Handles the device back action for this screen. */
        if (Platform.OS === 'web') return;
        const listener = BackHandler.addEventListener('hardwareBackPress',  () => { previous(); return true; });
        /** Removes the registered listeners when the effect ends. */
        return  () => listener.remove();
    }, [go, step]);
    /** Creates the swipe responder for introductory screen navigation. */
    const pan = useMemo( () => PanResponder.create({
        /** Checks whether the gesture is a horizontal swipe. */
        onMoveShouldSetPanResponder:  (_, gesture) => Math.abs(gesture.dx) > 30 && Math.abs(gesture.dx) > Math.abs(gesture.dy) * 2,
        /** Opens the next or previous introduction after a swipe. */
        onPanResponderRelease:  (_, gesture) => { if (gesture.dx < -55) next(); else if (gesture.dx > 55) previous(); },
    }), [go, step]);
    /** Displays bundled vector artwork at the calculated screen scale. */
    const vector =  (source: BundledAsset, w: number, h: number, targetWidth: number, targetHeight: number, testID: string) => <View pointerEvents="none" testID={testID} style={{ width: targetWidth, height: targetHeight }}><View style={{ width: w, height: h, transformOrigin: 'top left', transform: [{ scaleX: targetWidth / w }, { scaleY: targetHeight / h }] }}><LocalVector source={source} width={w} height={h}/></View></View>;
    return <View style={styles.safe}><ScrollView bounces={false} contentContainerStyle={{ flexGrow: 1 }}>
        <View {...pan.panHandlers} testID={`intro-screen-${step + 1}`} style={{ width: canvasWidth, height: canvasHeight, alignSelf: 'center', overflow: 'hidden', backgroundColor: '#000' }}>
            <Image testID="intro-landscape" source={image} style={{ position: 'absolute', width: canvasWidth, height: canvasHeight }} resizeMode="cover"/>
            <VignettePair height={canvasHeight}/>
            <View style={{ position: 'absolute', top: -.4 * scale, opacity: 0.6 }}>{vector({uri:'/assets/onboarding/0a3e3.svg'}, 409.194, 183.735, canvasWidth, canvasHeight * .2066 + .8 * scale, 'intro-upper-contours')}</View>
            <View style={{ position: 'absolute', top: canvasHeight * .656 - .4 * scale, opacity: 0.6 }}>{vector({uri:'/assets/onboarding/b18a2.svg'}, 409.194, 305.376, canvasWidth, canvasHeight * .344 + .8 * scale, 'intro-lower-contours')}</View>
            <View accessibilityLabel={`Introduction ${step + 1} of 4`} style={{ position: 'absolute', top: canvasHeight * .7254, alignSelf: 'center', flexDirection: 'row', gap: 8 * scale, alignItems: 'center' }}>{pages.map(
                /** Renders a item entry from its data. */
                 (_, index) => index === step ? <View key={index} style={{ width: (step === 0 ? 24 : 20) * scale, height: 7 * scale, borderRadius: 4 * scale, backgroundColor: '#dfa94a' }}/> : <View key={index}>{vector(step === 0 ? {uri:'/assets/onboarding/74d7e.svg'} : {uri:'/assets/onboarding/93445.svg'}, 7, 7, 7 * scale, 7 * scale, `intro-dot-${index + 1}`)}</View>)}</View>
            <Text accessibilityRole="header" style={{ position: 'absolute', top: canvasHeight * ((step === 3 ? 700.22 : 698.22) / 885.372), left: 0, width: '100%', textAlign: 'center', color: '#fff', fontSize: 26 * scale, lineHeight: 32 * scale, opacity:1,zIndex:2, fontFamily: 'Inter', fontWeight: '700' }}>{title}</Text>
            <Text style={{ position: 'absolute', top: canvasHeight * (step === 3 ? .8394 : .8372), left: canvasWidth * copyLeft, color: '#fff', fontSize: 16 * scale, lineHeight: 19 * scale, fontFamily: 'Inter' }}>{description}</Text>
            {step === 3 && <Pressable accessibilityRole="button" accessibilityLabel="Next to sign in" onPress={next} style={
                /** Returns the control styles for its current pressed state. */
                 ({ pressed }) => ({ position: 'absolute', top: Math.min(canvasHeight * (813.22 / 885.372), canvasHeight - insets.bottom - 52 * scale), right: 28 * scale, minWidth: 92 * scale, minHeight: 44, flexDirection: 'row', gap: 5 * scale, alignItems: 'center', justifyContent: 'center', opacity: pressed ? .7 : 1 })}>
                <Text style={{ fontFamily: 'Inter', fontWeight: '700', color: '#d4af53', fontSize: 20 * scale }}>NEXT</Text>
                <ChevronRight size={24 * scale} color="#ffffff"/>
            </Pressable>}
            <View testID="intro-logo" accessible accessibilityLabel="Level Up — Skills, People, Opportunities" style={{ position: 'absolute', opacity: 1, left: canvasWidth * .2821, top: Math.max(canvasHeight * .0883, insets.top + 12), width: 175 * scale, height: 175 * scale }}>
                <View style={{ position: 'absolute', left: 28.0525 * scale, top: 18 * scale }}>{vector({uri:'/assets/onboarding/414ef.svg'}, 116.946, 89.6636, 116.946 * scale, 89.6636 * scale, 'intro-logo-disc')}</View>
                <Image source={{uri:'/assets/onboarding/1d544.png'}} style={{ width: 175 * scale, height: 175 * scale }}/>
            </View>
        </View>
    </ScrollView></View>;
}
const styles = StyleSheet.create({ safe: { flex: 1, backgroundColor: '#000' } });
