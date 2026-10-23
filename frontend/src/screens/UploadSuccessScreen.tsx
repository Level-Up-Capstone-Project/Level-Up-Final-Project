/** Displays confirmation that a document has been added. */
import FeedbackLayout from '../feedback/FeedbackLayout';
import type {FeedbackProps} from '../feedback/types';
/** Displays confirmation that a document has been added. */
export default function UploadSuccessScreen(props:FeedbackProps){return <FeedbackLayout {...props} title='DOCUMENT ADDED' description='Your document has been added on this device and is ready for the next step.' button='Continue'/>;}
