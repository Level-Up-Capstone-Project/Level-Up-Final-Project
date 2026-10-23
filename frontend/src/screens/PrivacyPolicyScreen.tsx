/** Displays the Privacy Policy using the shared legal document layout. */
import LegalDocument from '../legal/LegalDocument';
import type { LegalScreenProps } from '../legal/types';
/** Displays the Privacy Policy using the shared legal document layout. */
export default function PrivacyPolicyScreen({onBack,onRelated}:LegalScreenProps) {
    return <LegalDocument document="privacy" onBack={onBack} onRelated={onRelated}/>;
}
