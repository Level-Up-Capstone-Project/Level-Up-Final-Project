/** Displays the MIRA conversation, suggested prompts and message input. */
import VignettePair from '../components/VignettePair';
import ScreenHeader, {screenHeaderTop} from '../components/ScreenHeader';
import { useEffect, useRef } from 'react';
import { BackHandler, Image, Keyboard, KeyboardAvoidingView, Platform, Pressable, ScrollView, StyleSheet, TextInput, useWindowDimensions, View } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import Svg, { Defs, LinearGradient, Rect, Stop } from 'react-native-svg';
import Text from '../components/Text';
import ChevronRight from '../components/ChevronRight';
import UploadVector from '../upload/UploadVector';
import type { AppModel } from '../navigation/useAppModel';
import MessageText from '../chat/MessageText';

/** Displays the MIRA conversation, suggested prompts and message input. */
export default function MiraChatScreen({ model }: { model: AppModel }) {
    const { width, height } = useWindowDimensions();
    const insets = useSafeAreaInsets();
    const scale = width / 408.393;
    const canvasHeight = Math.max(885.372, height / scale);
    const conversation = useRef<ScrollView>(null);
    const page = useRef<ScrollView>(null);
    const input = useRef<TextInput>(null);
    const { messages, draft, setDraft, sending, send } = model.mira;
    useEffect(
        /** Registers the hardware back action and its cleanup. */
         () => {
        /** Handles the device back action for this screen. */
        if (Platform.OS === 'web') return;
        const back = BackHandler.addEventListener('hardwareBackPress',  () => { model.goBack('home'); return true; });
        /** Handles the registered device or application event. */
        const keyboard = Keyboard.addListener('keyboardDidShow',  () => page.current?.scrollToEnd({ animated: true }));
        /** Removes the registered listeners when the effect ends. */
        return  () => { back.remove(); keyboard.remove(); };
    }, [model.goBack]);
    return <KeyboardAvoidingView style={styles.safe} behavior={Platform.OS === 'ios' ? 'padding' : undefined}>

        <ScrollView ref={page} keyboardShouldPersistTaps="handled" contentContainerStyle={{ flexGrow: 1, alignItems: 'center' }}>
            <View testID="mira-screen" style={{ width: 408.393 * scale, height: canvasHeight * scale, overflow: 'hidden' }}>
                <View style={{ width: 408.393, height: canvasHeight, transformOrigin: 'top left', transform: [{ scale }], backgroundColor: '#000' }}>
                    <View pointerEvents="none" style={StyleSheet.absoluteFill}>
                        <Image testID="mira-landscape" source={{uri:'/assets/mira/landscape.png'}} resizeMode="stretch" style={{ position: 'absolute', width: 863.445, height: 485.981, left: -207.423, top: -19.213 }}/>
                        <VignettePair height={canvasHeight}/>
                        <View testID="mira-upper-contours" style={{ position: 'absolute', left: .22, top: -.18, opacity: .35 }}><UploadVector source={{uri:'/assets/mira/upper-contours.svg'}} width={408.8} height={183.658} size={408}/></View>
                        <View testID="mira-lower-contours" style={{ position: 'absolute', left: .22, top: canvasHeight - 305.249, opacity: .35 }}><UploadVector source={{uri:'/assets/mira/lower-contours.svg'}} width={408.8} height={305.249} size={408}/></View>
                        <Svg width={408.393} height={340} style={StyleSheet.absoluteFill}><Defs><LinearGradient id="mira-contour-shade" x1="0" y1="0" x2="0" y2="1"><Stop offset="0" stopColor="#000" stopOpacity={.504}/><Stop offset="0.6875" stopColor="#000" stopOpacity={.266}/><Stop offset="1" stopColor="#000" stopOpacity={0}/></LinearGradient></Defs><Rect width={408.393} height={340} fill="url(#mira-contour-shade)"/></Svg>
                    </View>
                    <View style={[styles.foreground, { paddingTop: screenHeaderTop(scale,insets.top)/scale, paddingBottom: Math.max(26.152, insets.bottom / scale + 12) }]}>
                        <ScreenHeader title="MIRA CHAT BOT" onBack={
                            /** Opens the home screen. */
                             ()=>model.goBack('home')} backLabel="Back to previous page" style={{marginBottom:27.22}}/>
                        <View style={[styles.card, styles.welcome]}><Text style={styles.welcomeText}>{'I’m here to help you with career advice, skill development, opportunities and more.\n\nWhat would you like to work on today?'}</Text></View>
                        <ScrollView ref={conversation} testID="mira-conversation" keyboardShouldPersistTaps="handled" style={styles.conversation} contentContainerStyle={{ gap: 12, paddingVertical: 14 }} onContentSizeChange={
                            /** Handles the on content size change for this component. */
                             () => conversation.current?.scrollToEnd({ animated: true })}>
                            {messages.map(
                                /** Renders a message entry from its data. */
                                 message => <View key={message.id} style={[styles.bubble, message.role === 'user' && styles.userBubble]}>
                                <Text style={styles.author}>{message.role === 'user' ? 'You' : 'MIRA'}</Text>{message.role === 'assistant' ? <MessageText text={message.text} style={styles.message}/> : <Text selectable style={styles.message}>{message.text}</Text>}
                            </View>)}
                            {sending && <Text accessibilityLiveRegion="polite" style={styles.typing}>MIRA is typing…</Text>}
                        </ScrollView>
                        {!!model.mira.error && <Text accessibilityRole="alert" style={[styles.typing,{marginBottom:12}]}>{model.mira.error}</Text>}<View style={styles.suggestions}>
                            <Pressable accessibilityRole="button" disabled={sending} accessibilityState={{ disabled: sending }} onPress={
                                /** Submits the selected prompt or the current message draft. */
                                 () => send('Suggest learning resources')} style={
                                /** Returns the control styles for its current pressed state. */
                                 ({ pressed }) => [styles.card, styles.suggestion, pressed && { opacity: .7 }]}>
                                <Image testID="mira-learning" source={{uri:'/assets/mira/learning.png'}} resizeMode="contain" style={styles.ideas}/>
                                <Text style={styles.suggestionText}>Suggest learning resources</Text><ChevronRight color="#fff"/></Pressable>
                            <Pressable accessibilityRole="button" disabled={sending} accessibilityState={{ disabled: sending }} onPress={
                                /** Updates input focus and its visual state. */
                                 () => { send('Explain a topic'); input.current?.focus(); }} style={
                                /** Returns the control styles for its current pressed state. */
                                 ({ pressed }) => [styles.card, styles.suggestion, pressed && { opacity: .7 }]}>
                                <Image testID="mira-ideas" source={{uri:'/assets/mira/ideas.png'}} resizeMode="contain" style={styles.ideas}/><Text style={styles.suggestionText}>Explain a topic</Text><ChevronRight color="#fff"/>
                            </Pressable>
                        </View>
                        <View style={[styles.card, styles.composer]}>
                            <TextInput ref={input} accessibilityLabel="Message MIRA" placeholder="Ask MIRA (Mission Intelligence & Readiness Assistant)" placeholderTextColor="#696967" value={draft} onChangeText={setDraft} multiline maxLength={2000} returnKeyType="send" submitBehavior="submit" blurOnSubmit onSubmitEditing={
                                /** Submits the selected prompt or the current message draft. */
                                 () => send()} style={[styles.input, !!draft.trim() && { paddingRight: 44 }]}/>
                            {!!draft.trim() && <Pressable accessibilityRole="button" accessibilityLabel="Send message" accessibilityState={{ disabled: sending }} disabled={sending} onPress={
                                /** Submits the selected prompt or the current message draft. */
                                 () => send()} style={
                                /** Returns the control styles for its current pressed state. */
                                 ({ pressed }) => [styles.send, { opacity: sending || pressed ? .5 : 1 }]}><UploadVector source={{uri:'/assets/mira/send.svg'}} width={18}/></Pressable>}
                        </View>
                    </View>
                </View>
            </View>
        </ScrollView>
    </KeyboardAvoidingView>;
}

const bodyFont = Platform.select({ web: 'Arial', android: 'Roboto', ios: 'System' });
const styles = StyleSheet.create({
    safe: { flex: 1, backgroundColor: '#000' },
    foreground: { flex: 1, paddingHorizontal: 23.22 },
    header: { height: 40, marginBottom: 24, justifyContent: 'center' },
    back: { position: 'absolute', left: -5, top: 0, width: 44, height: 44, justifyContent: 'center', alignItems: 'center' },
    title: { fontSize: 32, lineHeight: 39, fontWeight: '700', color: '#f8f9f5', opacity: .7, marginLeft: 46, letterSpacing: -.3 },
    card: { backgroundColor: 'rgba(6,15,20,.85)', borderWidth: 1, borderColor: '#353e44', borderRadius: 10 },
    welcome: { height: 130, marginLeft: -2, marginRight: 2, paddingTop: 15, paddingHorizontal: 21 },
    welcomeText: { fontFamily: bodyFont, fontSize: 15, lineHeight: 22, letterSpacing: .4, color: '#a6a6a4' },
    conversation: { flex: 1 },
    bubble: { alignSelf: 'flex-start', maxWidth: '94%', padding: 14, borderRadius: 10, backgroundColor: 'rgba(6,15,20,.94)', borderWidth: 1, borderColor: '#353e44' },
    userBubble: { alignSelf: 'flex-end', backgroundColor: '#252112', borderColor: '#765728' },
    author: { fontSize: 10, color: '#e7b663', marginBottom: 7 },
    message: { fontFamily: bodyFont, fontSize: 14, lineHeight: 21, color: '#eee', letterSpacing: .2 },
    typing: { color: '#a6a6a4', fontSize: 12, paddingHorizontal: 10 },
    suggestions: { gap: 7 },
    suggestion: { height: 66, flexDirection: 'row', alignItems: 'center', paddingLeft: 63, paddingRight: 15, justifyContent: 'space-between', gap: 6 },
    suggestionText: { fontFamily: bodyFont, fontSize: 18, fontWeight: '400', lineHeight: 22, letterSpacing: .4, color: '#fff', flexShrink: 1 },
    ideas: { position: 'absolute', width: 38, height: 38, left: 14.9, top: 13.1 },
    composer: { height: 106, marginTop: 20, paddingHorizontal: 14, paddingTop: 8, paddingBottom: 12 },
    input: { flex: 1, fontFamily: bodyFont, fontSize: 12, lineHeight: 22, letterSpacing: .4, color: '#fff', textAlignVertical: 'top', padding: 0, outlineWidth: 0 },
    send: { position: 'absolute', right: 9, bottom: 7, width: 40, height: 36, alignItems: 'center', justifyContent: 'center', borderRadius: 8, backgroundColor: '#18242a' },
});
