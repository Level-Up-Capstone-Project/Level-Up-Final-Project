/** Displays confirmation of a successful password reset. */
import FeedbackLayout from '../feedback/FeedbackLayout';
import type {FeedbackProps} from '../feedback/types';
/** Displays confirmation of a successful password reset. */
export default function PasswordResetSuccessScreen(props:FeedbackProps){return <FeedbackLayout {...props} title='PASSWORD RESET' description='Your password has been reset. Sign in with your new password.' button='Back to Sign In'/>;}
