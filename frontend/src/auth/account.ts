/** Defines account details, authentication screen names and contact validation. */
import type { WorkArea } from '../location/types';
export type AuthScreen = 'signin' | 'signup' | 'forgot' | 'otp' | 'reset';
export type Account = {
    name: string;
    email: string;
    firstName?: string;
    lastName?: string;
    qualification?: string;
    fieldOfStudy?: string;
    experienceLevel?: string;
    interests?: string;
    targetCareerID?: string;
    industry?: string;
    targetRoles?: string[];
    phone?: string;
    location?: string;
    workAreas?: WorkArea[];
    password: string;
    token?: string;
};

export const defaultAccount: Account = { name: '', email: '', password: '' };
/** Checks whether a contact value has an email or phone-number format. */
export function isValidContact(value: string) {
    const contact = value.trim();
    return /^\S+@\S+\.\S+$/.test(contact) || /^\+?[\d\s()-]{8,18}$/.test(contact);
}
