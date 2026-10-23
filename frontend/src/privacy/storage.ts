/** Reads and writes account consent records in browser storage. */
import type {Consent} from './types';
import {documentVersion} from './types';
/** Builds the browser storage key for the account consent record. */
const key= (owner:string)=>'career-path-consent-'+(owner.toLowerCase()||'anonymous');
/** Reads the account consent record from device storage. */
export function loadConsent(owner:string):Consent|null {try{const value=JSON.parse(localStorage.getItem(key(owner))||'null');return value&&(value.status==='granted'||value.status==='withdrawn')&&value.documentVersion===documentVersion?value:null;}catch{return null;}}
/** Writes the account consent choice to device storage. */
export function saveConsent(owner:string,value:Consent):boolean {try{localStorage.setItem(key(owner),JSON.stringify(value));return true;}catch{return false;}}
