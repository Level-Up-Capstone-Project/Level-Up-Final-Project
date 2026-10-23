/** Displays confirmation that consent has been withdrawn. */
import FeedbackLayout from '../feedback/FeedbackLayout';
import type {FeedbackProps} from '../feedback/types';
/** Displays confirmation that consent has been withdrawn. */
export default function ConsentWithdrawnScreen(props:FeedbackProps){return <FeedbackLayout {...props} title='CONSENT WITHDRAWN' description='Your consent has been withdrawn on this device. Document analysis and personalisation are paused. This does not delete your account or files.' button='Back to Settings'/>;}
