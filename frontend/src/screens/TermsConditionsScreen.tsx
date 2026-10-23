/** Displays Terms & Conditions using the shared legal document layout. */
import LegalDocument from '../legal/LegalDocument';
import type { LegalScreenProps } from '../legal/types';
/** Displays Terms & Conditions using the shared legal document layout. */
export default function TermsConditionsScreen({onBack,onRelated}:LegalScreenProps) {
    return <LegalDocument document="terms" onBack={onBack} onRelated={onRelated}/>;
}
