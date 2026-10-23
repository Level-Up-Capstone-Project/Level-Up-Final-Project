/** Defines consent status, its timestamp and the document version. */
export type Consent = {status:'granted'|'withdrawn';updatedAt:string;documentVersion:string};
export const documentVersion='2026-10-23';
