import {apiRequest} from './client';
import type {WorkArea} from '../location/types';

export type Profile = {
    profileID: string;
    userID: string;
    targetCareerID: string | null;
    fName: string;
    lName: string;
    qualification: string;
    fieldOfStudy: string;
    experienceLevel: string | null;
    interests: string | null;
    industry: string | null;
    phone: string | null;
    targetRoles: string[];
    workAreas: WorkArea[];
    profileCompleted: boolean;
    updatedAt: string;
};

export type ProfileUpdate = {
    fName: string;
    lName: string;
    qualification: string;
    fieldOfStudy: string;
    experienceLevel?: string;
    interests?: string;
    industry?: string | null;
    phone?: string | null;
    targetRoles?: string[];
    workAreas?: WorkArea[];
    targetCareerID?: string | null;
};

export type ProfilePreferences = Pick<ProfileUpdate, 'industry' | 'phone' | 'targetRoles' | 'workAreas'>;

export const loadProfile = (token: string, signal?: AbortSignal) =>
    apiRequest<Profile>('/api/profile', {token, signal});

export const saveProfile = (token: string, profile: ProfileUpdate, signal?: AbortSignal) =>
    apiRequest<Profile>('/api/profile', {method: 'PUT', token, body: profile, signal});

export const saveProfilePreferences = (token: string, preferences: ProfilePreferences, signal?: AbortSignal) =>
    apiRequest<Profile>('/api/profile/preferences', {method: 'PUT', token, body: preferences, signal});
