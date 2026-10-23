/** Provides the account recovery contact form. */
import { View } from 'react-native';
import AuthLayout from '../components/AuthLayout';
import Field from '../components/Field';
import PrimaryAction from '../components/PrimaryAction';
import { useAuthStyles } from '../styles';
import type { AuthModel } from '../useAuthModel';
/** Provides the account recovery contact form. */
export default function ForgotPasswordScreen({ model }: {
    model: AuthModel;
}) {
    const styles = useAuthStyles();
    const { recoveryContact, setRecoveryContact, startRecovery } = model;
    return <AuthLayout model={model}><><View style={styles.recoveryFields}><Field label="Email address" value={recoveryContact} onChangeText={setRecoveryContact} keyboardType="email-address" autoComplete="email" editable={!model.submitting} onSubmitEditing={startRecovery}/></View><PrimaryAction label="Send recovery code" onPress={startRecovery} loading={model.submitting}/></></AuthLayout>;
}
