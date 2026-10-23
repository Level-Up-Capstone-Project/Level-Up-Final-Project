/** Manages the current account consent choice and storage feedback. */
import {useState} from 'react';
import {loadConsent,saveConsent} from './storage';
import {documentVersion,type Consent} from './types';
/** Manages the current account consent choice and storage feedback. */
export default function useConsent(owner:string){
 const [state,setState]=useState<{owner:string;value:Consent|null}>({owner,value:loadConsent(owner)});
 const consent=state.owner===owner?state.value:loadConsent(owner);
 const [error,setError]=useState('');
 /** Updates consent status and saves the device-local record. */
 const update= (status:Consent['status'],consentOwner=owner)=>{const value={status,updatedAt:new Date().toISOString(),documentVersion};const saved=saveConsent(consentOwner,value);setState({owner:consentOwner,value});setError(saved?'':'Your choice applies for this session, but could not be saved on this device. Please try again.');return saved;};
 return {consent,error,
 /** Records agreement to the policy documents. */
 grant: (consentOwner=owner)=>update('granted',consentOwner),
 /** Records withdrawal of the current consent choice. */
 withdraw: ()=>update('withdrawn')};
}
