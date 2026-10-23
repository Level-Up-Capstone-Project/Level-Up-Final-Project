/** Displays the shared document-upload layout for an academic transcript. */
import DocumentUploadScreen from '../upload/DocumentUploadScreen';
import type { AppModel } from '../navigation/useAppModel';

/** Displays the shared document-upload layout for an academic transcript. */
export default function UploadTranscriptScreen({ model }: { model: AppModel }) {
    return <DocumentUploadScreen model={model} kind="transcript"/>;
}
