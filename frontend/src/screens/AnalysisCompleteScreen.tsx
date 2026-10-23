/** Displays confirmation that document analysis has completed. */
import FeedbackLayout from '../feedback/FeedbackLayout';
import type {FeedbackProps} from '../feedback/types';
/** Displays confirmation that document analysis has completed. */
export default function AnalysisCompleteScreen(props:FeedbackProps){return <FeedbackLayout {...props} title='ANALYSIS COMPLETE' description='Your document analysis has finished. You can now review the results.' button={props.nextScreen==='uploadTranscript'?'Add Transcript':'View Results'}/>;}
