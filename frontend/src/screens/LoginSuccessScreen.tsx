/** Displays a successful sign-in confirmation. */
import FeedbackLayout from '../feedback/FeedbackLayout';
import type {FeedbackProps} from '../feedback/types';
/** Displays a successful sign-in confirmation. */
export default function LoginSuccessScreen(props:FeedbackProps){return <FeedbackLayout {...props} title='LOGIN SUCCESSFUL' description='You are signed in. Your career journey continues here.' button='Go to Home'/>;}
