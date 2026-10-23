/** Selects the authentication screen and supplies its shared form state. */
import useAuthModel, { type AuthFlowProps } from './useAuthModel';
import SignInScreen from './screens/SignInScreen';
import SignUpScreen from './screens/SignUpScreen';
import ForgotPasswordScreen from './screens/ForgotPasswordScreen';
import EnterOtpScreen from './screens/EnterOtpScreen';
import ResetPasswordScreen from './screens/ResetPasswordScreen';
const screens = { signin: SignInScreen, signup: SignUpScreen, forgot: ForgotPasswordScreen, otp: EnterOtpScreen, reset: ResetPasswordScreen };
/** Selects the authentication screen and supplies its shared form state. */
export default function AuthFlow(props: AuthFlowProps) {
    const model = useAuthModel(props);
    const ScreenComponent = screens[props.screen];
    return <ScreenComponent model={model}/>;
}
