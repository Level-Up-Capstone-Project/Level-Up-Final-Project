/** Arranges authentication headings, descriptions, forms and recovery steps. */
import ScreenHeader, {screenHeaderTop} from '../../components/ScreenHeader';
import React from 'react';
import { KeyboardAvoidingView, Platform, Pressable, ScrollView, Text, View } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import AuthIcon from './AuthIcon';
import AuthBackground from './AuthBackground';

import { useAuthStyles } from '../styles';
import type { AuthModel } from '../useAuthModel';
const description = 'Enter your registered email address to reset your password.';
/** Arranges authentication headings, descriptions, forms and recovery steps. */
export default function AuthLayout({ model, children }: {
    model: AuthModel;
    children: React.ReactNode;
}) {
    const styles = useAuthStyles();
    const scale = Math.min(model.width, 480) / 408.393;
    const { screen, loadedAssets, assetError, signup, signin, recovery, name, submitting, message, scroll, back, title, minHeight } = model;
    const darkDesign = true;
    if (!loadedAssets && !assetError)
        return <View style={styles.safe}/>;
    return <SafeAreaView style={[styles.safe,darkDesign && {backgroundColor:'#000'}]} edges={['top', 'bottom']}>
    <KeyboardAvoidingView style={styles.flex} behavior={Platform.OS === 'ios' ? 'padding' : undefined}>
      <ScrollView ref={scroll} keyboardShouldPersistTaps="handled" contentContainerStyle={[styles.canvas, { minHeight, maxWidth: 480, paddingBottom: 32 * scale }]}>

        {darkDesign && <AuthBackground width={Math.min(model.width,480)} height={Math.max(model.height,885.372*scale)} top={-model.insets.top}/>}
        <View style={[styles.body, { paddingHorizontal: 23 * scale,zIndex:1 }]}>
          <View style={[styles.header,{marginTop:screenHeaderTop(scale,model.insets.top)-model.insets.top,zIndex:2,opacity:1}]}>
            <ScreenHeader title={title.toUpperCase()} onBack={recovery?back:undefined} backLabel="Go back" disabled={submitting} scale={scale}/>
            <Text style={[styles.description,(signin || signup || screen === 'forgot' || screen === 'reset') && {color:'#a6a6a4'},recovery && {minHeight:44 * scale}]}>{signin ? 'Your journey continues here. Pick up where you left off, strengthen your skills and take the next step towards your future.' : signup ? 'Discover your strengths, sharpen your skills and create your own path towards a future filled with opportunities.' : screen === 'otp' ? model.passwordRecovery ? 'Enter the six-digit code sent to your email to continue resetting your password.' : 'Enter the OTP code we just sent\nyou on your registered email address.' : screen === 'reset' ? 'Type in the new password you wish to use.' : description}</Text>
          </View>
          {(!!message || !!assetError) && <Text accessibilityRole="alert" accessibilityLiveRegion="polite" style={styles.message}>{assetError ? 'Could not load the screen icons. Please reload the app.' : message}</Text>}
          {children}
        </View>
        {recovery && <View accessibilityLabel={`Recovery step ${screen === 'forgot' ? 1 : screen === 'otp' ? 2 : 3} of 3`} style={[styles.progress, { bottom: Math.max(24, 87.15 * scale - model.insets.bottom) }]}>{[0, 1, 2].map(
            /** Renders a item entry from its data. */
             step => <View key={step} style={[styles.progressSegment, step === (screen === 'forgot' ? 0 : screen === 'otp' ? 1 : 2) && styles.progressActive]}/>)}</View>}
      </ScrollView>
    </KeyboardAvoidingView>
  </SafeAreaView>;
}
