/** Opens Terms & Conditions or Privacy Policy in a full-screen modal. */
import { useEffect, useState } from 'react';
import { Modal } from 'react-native';
import TermsConditionsScreen from '../screens/TermsConditionsScreen';
import PrivacyPolicyScreen from '../screens/PrivacyPolicyScreen';
import type { LegalDocumentType } from './types';

/** Opens Terms & Conditions or Privacy Policy in a full-screen modal. */
export default function LegalModal({document,onClose}:{document:LegalDocumentType|null;onClose:()=>void}) {
    const [current,setCurrent]=useState(document);
    useEffect(
        /** Synchronises component state with the effect dependencies. */
         ()=>setCurrent(document),[document]);
    if(!document)return null;
    const Screen=(current || document)==='terms'?TermsConditionsScreen:PrivacyPolicyScreen;
    return <Modal visible animationType="slide" presentationStyle="fullScreen" onRequestClose={onClose}><Screen onBack={onClose} onRelated={setCurrent}/></Modal>;
}
