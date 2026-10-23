/** Displays confirmation that the local session has ended. */
import FeedbackLayout from '../feedback/FeedbackLayout';
import type {FeedbackProps} from '../feedback/types';
/** Displays confirmation that the local session has ended. */
export default function SignedOutScreen(props:FeedbackProps){return <FeedbackLayout {...props} title='SIGNED OUT' description='You have been signed out of this device.' button='Back to Sign In'/>;}
