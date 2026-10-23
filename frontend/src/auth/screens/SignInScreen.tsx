/** Provides sign-in fields and account recovery links. */
import { Pressable, Text, View } from 'react-native';
import AuthLayout from '../components/AuthLayout';
import AuthIcon from '../components/AuthIcon';
import Field from '../components/Field';
import PrimaryAction from '../components/PrimaryAction';
import { useAuthStyles } from '../styles';
import type { AuthModel } from '../useAuthModel';
/** Provides sign-in fields and account recovery links. */
export default function SignInScreen({ model }: {
    model: AuthModel;
}) {
    const styles = useAuthStyles();
    const scale = Math.min(model.width, 480) / 408.393;
    const { navigate, onAuthenticated, compact, contact, setContact, password, setPassword, setRecoveryContact, signIn } = model;
    return <AuthLayout model={model}><>
            <View style={[styles.socialRow, { marginTop: 17 * scale }, compact && { gap: 12 }]}>{(['google'] as const).map(
                /** Renders a item entry from its data. */
                 provider => <Pressable key={provider} disabled={model.submitting} accessibilityState={{disabled: model.submitting, busy: model.submitting}} accessibilityRole="button" accessibilityLabel={`Continue with ${'Google'}`} onPress={
                /** Displays feedback for the selected action. */
                 model.googleSignIn} style={[styles.social, compact && { paddingHorizontal: 10, gap: 8 }]}>
              <View style={styles.socialIcon}><AuthIcon name={provider}/></View><Text style={[styles.socialText, compact && { fontSize: 14 }]}>{'Google'}</Text>
            </Pressable>)}</View>
            <View style={styles.divider}><View style={styles.dividerClip}><AuthIcon name="divider"/></View><Text style={styles.or}>Or</Text><View style={styles.dividerClip}><AuthIcon name="divider"/></View></View>
            <View style={styles.fields}>

              <Field label="Email Address" value={contact} onChangeText={setContact} keyboardType="email-address" autoComplete="email"/>
              <Field label="Password" value={password} onChangeText={setPassword} secure signup={false} autoComplete={'current-password'} onSubmitEditing={signIn}/>
            </View>
            <Pressable accessibilityRole="button" onPress={
                /** Handles the on press for this component. */
                 () => { setRecoveryContact(contact); navigate('forgot'); }} style={styles.forgot}><Text style={styles.forgotText}>Forgot Password?</Text></Pressable>
            <PrimaryAction textColor="#ebe1d6" label={'Log In'} onPress={signIn} loading={model.submitting} marginTop={39 * scale}/>
            <Text style={[styles.accountSwitch,{color:'#a6a6a4'}]}>{'Don’t have account? '}<Text accessibilityRole="link" style={styles.link} onPress={
                /** Handles the on press for this component. */
                 () => navigate('signup')}>{'Sign Up'}</Text></Text>
          </></AuthLayout>;
}
