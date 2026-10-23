/** Provides the new-password and confirmation form. */
import { View } from 'react-native';
import AuthLayout from '../components/AuthLayout';
import Field from '../components/Field';
import PrimaryAction from '../components/PrimaryAction';
import { useAuthStyles } from '../styles';
import type { AuthModel } from '../useAuthModel';
/** Provides the new-password and confirmation form. */
export default function ResetPasswordScreen({ model }: {
    model: AuthModel;
}) {
    const styles = useAuthStyles();
    const { newPassword, setNewPassword, confirmation, setConfirmation, submitting, submitReset } = model;
    return <AuthLayout model={model}><><View style={[styles.recoveryFields, styles.fields]}><Field label="New password" secure value={newPassword} onChangeText={setNewPassword} autoComplete="new-password" editable={!submitting}/><Field label="Confirm password" secure value={confirmation} onChangeText={setConfirmation} autoComplete="new-password" editable={!submitting} onSubmitEditing={submitReset}/></View><PrimaryAction label="Continue" onPress={submitReset} loading={submitting}/></></AuthLayout>;
}
