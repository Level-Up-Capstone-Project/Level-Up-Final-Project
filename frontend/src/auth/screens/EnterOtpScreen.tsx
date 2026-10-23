/** Provides the verification-code entry form. */
import { Text, TextInput, View } from 'react-native';
import AuthLayout from '../components/AuthLayout';
import PrimaryAction from '../components/PrimaryAction';
import { useAuthStyles, colors } from '../styles';
import type { AuthModel } from '../useAuthModel';
/** Provides the verification-code entry form. */
export default function EnterOtpScreen({ model }: {
    model: AuthModel;
}) {
    const styles = useAuthStyles();
    const scale = Math.min(model.width, 480) / 408.393;
    const { otp, setOtp, setMessage, resendSeconds, setResendSeconds, inputs, verify, changeOtp } = model;
    return <AuthLayout model={model}><>
            <View style={styles.otpRow}>{otp.map(
                /** Renders a item entry from its data. */
                 (digit, index) => <TextInput key={index} ref={
                /** Handles the ref for this component. */
                 input => { inputs.current[index] = input; }} accessibilityLabel={`OTP digit ${index + 1}`} value={digit} onChangeText={
                /** Handles the on change text for this component. */
                        value => changeOtp(value, index)} keyboardType="number-pad" autoComplete={index === 0 ? 'one-time-code' : 'off'} maxLength={index === 0 ? 6 : 1} selectTextOnFocus style={styles.otpInput} selectionColor={colors.primary} onKeyPress={
                /** Updates input focus and its visual state. */
                 ({ nativeEvent }) => {
                if (nativeEvent.key === 'Backspace' && !digit && index > 0)
                    inputs.current[index - 1]?.focus();
            }} onSubmitEditing={verify}/>)}</View>
        <PrimaryAction
            label={model.passwordRecovery ? 'Continue' : 'Verify Email'}
            onPress={verify} loading={model.submitting}
            marginTop={52 * scale}
        />
            <Text style={[styles.accountSwitch, { marginLeft: 24 * scale }]}>Didn’t get a code? <Text accessibilityRole="link" accessibilityState={{ disabled: resendSeconds > 0 || model.submitting }} style={styles.link} onPress={
                /** Updates input focus and its visual state. */
                 model.resendVerification}>{resendSeconds ? `Resend in ${resendSeconds}s` : 'Resend OTP'}</Text></Text>
          </></AuthLayout>;
}
