/** Provides a text input with focus styling and password visibility controls. */
import { useEffect, useRef, useState } from 'react';
import { AppState, Pressable, TextInput, type TextInputProps } from 'react-native';
import { useAuthStyles, colors } from '../styles';
import AuthIcon from './AuthIcon';
/** Provides a text input with focus styling and password visibility controls. */
export default function Field({ label, secure = false, concealable = false, signup = false, ...props }: TextInputProps & {
    label: string;
    secure?: boolean;
    concealable?: boolean;
    signup?: boolean;
}) {
    const styles = useAuthStyles();
    const input = useRef<TextInput>(null);
    const [visible, setVisible] = useState(!secure);
    const [focused, setFocused] = useState(false);
    useEffect(
        /** Synchronises component state with the effect dependencies. */
         () => {
        if (secure && !props.value) setVisible(false);
    }, [secure, props.value]);
    useEffect(
        /** Synchronises component state with the effect dependencies. */
         () => {
        if (!secure) return;
        /** Handles the registered device or application event. */
        const subscription = AppState.addEventListener('change',  state => {
            if (state !== 'active') setVisible(false);
        });
        /** Removes the registered listeners when the effect ends. */
        return  () => subscription.remove();
    }, [secure]);
    return <Pressable accessible={false} onPress={
        /** Updates input focus and its visual state. */
         () => { if (props.editable !== false) input.current?.focus(); }} style={[styles.field, focused && props.editable !== false && styles.focused]}>
    <TextInput {...props} ref={input} accessibilityLabel={label} placeholder={label} placeholderTextColor={colors.placeholder} style={[styles.input, secure && !!props.value && { fontFamily: 'PoppinsMedium' }]} secureTextEntry={(secure || concealable) && !visible} autoCorrect={secure ? false : props.autoCorrect} spellCheck={secure ? false : props.spellCheck} autoCapitalize={props.autoCapitalize ?? "none"} selectionColor={colors.primary} onFocus={
        /** Updates input focus and its visual state. */
         event => { setFocused(true); props.onFocus?.(event); }} onBlur={
        /** Updates input focus and its visual state. */
         event => { setFocused(false); props.onBlur?.(event); }}/>
    {(secure || concealable) && <Pressable accessibilityRole="button" accessibilityLabel={`${visible ? 'Hide' : 'Show'} ${label.toLowerCase()}`} hitSlop={10} onPress={
        /** Handles the on press for this component. */
         event => { event.stopPropagation(); setVisible(!visible); }}>
      <AuthIcon name={signup ? 'signupEye' : 'eye'}/>
    </Pressable>}
  </Pressable>;
}
