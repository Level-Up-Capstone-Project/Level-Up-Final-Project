/** Defines the supported legal document identifiers. */
export type LegalDocumentType = 'terms' | 'privacy';
export type LegalScreenProps = {onBack:()=>void;onRelated?:(document:LegalDocumentType)=>void};
