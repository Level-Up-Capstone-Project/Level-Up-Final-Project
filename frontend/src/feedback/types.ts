/** Defines the supported feedback outcomes and their component properties. */
export type FeedbackKind = 'login'|'signedOut'|'upload'|'passwordReset'|'analysisComplete'|'consentWithdrawn';
export type FeedbackProps = {onContinue:()=>void;message?:string;nextScreen?:string};
