/** Displays the shared document-upload layout for a CV. */
import DocumentUploadScreen from '../upload/DocumentUploadScreen';
import type { AppModel } from '../navigation/useAppModel';

/** Displays the shared document-upload layout for a CV. */
export default function UploadScreen({ model }: { model: AppModel }) {
    return <DocumentUploadScreen model={model} kind="cv"/>;
}
