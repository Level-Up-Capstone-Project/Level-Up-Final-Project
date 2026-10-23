/** Collects account details, career preferences, work areas and policy agreement. */
import CareerPreferencesFields from '../../components/CareerPreferencesFields';
import WorkAreaPicker from '../../location/WorkAreaPicker';
import { Pressable, Text, View } from 'react-native';
import AuthLayout from '../components/AuthLayout';
import AuthIcon from '../components/AuthIcon';
import Field from '../components/Field';
import PrimaryAction from '../components/PrimaryAction';
import { useAuthStyles } from '../styles';
import type { AuthModel } from '../useAuthModel';
/** Collects account details, career preferences, work areas and policy agreement. */
export default function SignUpScreen({ model }: {
    model: AuthModel;
}) {
    const styles = useAuthStyles();
    const scale = Math.min(model.width, 480) / 408.393;
    const { navigate, onAuthenticated, compact, signup, workAreas, setWorkAreas, phone, setPhone, name, setName, contact, setContact, password, setPassword, agreed, setAgreed, signUp, policy } = model;
    return <AuthLayout model={model}><>
            <View style={[styles.socialRow, { marginTop: 17 * scale }, compact && { gap: 12 }]}>{(['google'] as const).map(
                /** Renders a item entry from its data. */
                 provider => <Pressable key={provider} disabled={model.submitting} accessibilityState={{disabled: model.submitting, busy: model.submitting}} accessibilityRole="button" accessibilityLabel={`Continue with ${'Google'}`} onPress={
                /** Displays feedback for the selected action. */
                 model.googleSignIn} style={[styles.social, compact && { paddingHorizontal: 10, gap: 8 }]}>
              <View style={styles.socialIcon}><AuthIcon name={provider}/></View><Text style={[styles.socialText, compact && { fontSize: 14 }]}>{'Google'}</Text>
            </Pressable>)}</View>
            <View style={styles.divider}><View style={styles.dividerClip}><AuthIcon name="signupDivider"/></View><Text style={styles.or}>Or</Text><View style={styles.dividerClip}><AuthIcon name="divider"/></View></View>
            <View style={styles.fields}>
              <Field label="Name and Surname" autoCapitalize="words" value={name} onChangeText={setName} autoComplete="name"/>
              <Field label="Email Address" value={contact} onChangeText={setContact} keyboardType="email-address" autoComplete="email"/>
              <Field label="Phone Number" value={phone} onChangeText={setPhone} keyboardType="phone-pad" autoComplete="tel" concealable signup/>
              <CareerPreferencesFields industry={model.industry} onIndustryChange={model.setIndustry} targetRoles={model.targetRoles} onRolesChange={model.setTargetRoles}/>
              <WorkAreaPicker value={workAreas} onChange={setWorkAreas}/>
              <Field label="Password" value={password} onChangeText={setPassword} secure signup={true} autoComplete={'new-password'} onSubmitEditing={signUp}/>
            </View>
            <View style={[styles.termsRow, { marginTop: 17 * scale, marginLeft: 4 * scale }]}><Pressable accessibilityRole="checkbox" accessibilityLabel="Agree to terms and privacy policy" accessibilityState={{ checked: agreed }} aria-checked={agreed} hitSlop={10} onPress={
                /** Applies the selected toggle or checkbox value. */
                 () => setAgreed(!agreed)} style={[styles.checkbox, agreed && styles.checked]}>{agreed && <Text style={styles.checkmark}>✓</Text>}</Pressable>
              <Text style={styles.terms}>I agree to the <Text accessibilityRole="link" style={styles.link} onPress={
                  /** Handles the on press for this component. */
                   () => policy('Terms of Service')}>Terms &amp; Conditions</Text> and <Text accessibilityRole="link" style={styles.link} accessibilityLabel="Privacy Policy" onPress={
                  /** Handles the on press for this component. */
                   () => policy('Privacy Policy')}>Privacy Policy</Text></Text>
            </View>
            <PrimaryAction label={'Create Account'} onPress={signUp} loading={model.submitting} marginTop={16 * scale}/>
            <Text style={[styles.accountSwitch,{color:'#a6a6a4'}]}>{'Already a Level Up member? '}<Text accessibilityRole="link" style={styles.link} onPress={
                /** Handles the on press for this component. */
                 () => navigate('signin')}>{'Sign in'}</Text></Text>
          </></AuthLayout>;
}
