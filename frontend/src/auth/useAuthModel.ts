/** Manages authentication form values, validation, recovery inputs and navigation. */
import { locationSummary, type WorkArea } from '../location/types';
import { useEffect, useRef, useState } from 'react';
import { Alert, BackHandler, Keyboard, Platform, ScrollView, TextInput, useWindowDimensions } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { authAssets } from './assets';
import { isValidContact, type AuthScreen, type Account } from './account';
import type { LegalDocumentType } from '../legal/types';
import { apiRequest, ApiError } from "../api/client";
import {saveProfilePreferences} from '../api/profile';
import {googleNeedsOnboarding, signInWithGoogle} from './google';

type RegisterResponse = {
    userID: string;
    email: string;
    isActive: boolean;
    token: string | null;
};

type VerifyEmailResponse = {
    message: string;
    token: string;
};
/** Manages authentication form values, validation, recovery inputs and navigation. */
export default function useAuthModel({ screen, navigate, onAuthenticated, onPasswordReset, account, updateAccount, openLegal }: {
    screen: AuthScreen;
    navigate: (screen: AuthScreen) => void;
    onAuthenticated: (signup?: boolean, email?: string) => void;
    onPasswordReset?: () => void;
    account: Account;
    updateAccount: (account: Account) => void;
    openLegal: (document: LegalDocumentType) => void;
}) {
    const loadedAssets=true,assetError=null;
    const { height, width } = useWindowDimensions();
    const compact = width < 360;
    const insets = useSafeAreaInsets();
    const signup = screen === 'signup';
    const signin = screen === 'signin';
    const recovery = !signup && !signin;
    const [industry,setIndustry] = useState('');
    const [targetRoles,setTargetRoles] = useState<string[]>([]);
    const [name, setName] = useState('');
    const [workAreas, setWorkAreas] = useState<WorkArea[]>([]);
    const location = locationSummary(workAreas);
    const [phone, setPhone] = useState('');
    const [contact, setContact] = useState('');
    const [password, setPassword] = useState('');
    const [recoveryContact, setRecoveryContact] = useState('');
    const [agreed, setAgreed] = useState(false);
    const [otp, setOtp] = useState(["", "", "", "", "", ""]);
    const [verified, setVerified] = useState(false);
    const [passwordRecovery, setPasswordRecovery] = useState(false);
    const [newPassword, setNewPassword] = useState('');
    const [confirmation, setConfirmation] = useState('');
    const [submitting, setSubmitting] = useState(false);
    const [message, setMessage] = useState('');
    const [resendSeconds, setResendSeconds] = useState(0);
    const inputs = useRef<Array<TextInput | null>>([]);
    const scroll = useRef<ScrollView>(null);
    const resetTimer = useRef<ReturnType<typeof setTimeout> | null>(null);
    const previous = useRef(screen);
    const googleRequest = useRef<AbortController | null>(null);
    useEffect(() => () => googleRequest.current?.abort(), [screen]);
    const googleSignIn = async () => {
        if (submitting) return;
        if (signup && !agreed) {setMessage('Please agree to the Terms & Conditions and Privacy Policy.'); return;}
        const controller = new AbortController();
        googleRequest.current = controller;
        setSubmitting(true); setMessage('');
        try {
            const result = await signInWithGoogle(controller.signal, signup ? 'signup' : 'signin');
            const sameAccount = account.email.toLowerCase() === result.login.user.email.toLowerCase();
            if (signup) await saveProfilePreferences(result.login.token, {
                industry: industry.trim(), phone: phone.trim(), targetRoles, workAreas,
            }, controller.signal);
            updateAccount({name: result.name, email: result.login.user.email, password: '', token: result.login.token,
                ...(sameAccount ? {...account, name: result.name, email: result.login.user.email, password: '', token: result.login.token} : {}),
                ...(signup ? {name: name.trim() || result.name, industry, phone, targetRoles, workAreas, location} : {}),
            });
            onAuthenticated(googleNeedsOnboarding(result), result.login.user.email);
        } catch (error) {
            if (!controller.signal.aborted) setMessage(error instanceof Error ? error.message : 'Google sign-in failed.');
        } finally {if (!controller.signal.aborted) setSubmitting(false); googleRequest.current = null;}
    };
    /** Returns to the previous authentication or recovery step. */
    const back =  () => {
        if (submitting)
            return;
        navigate(screen === 'forgot' ? 'signin' : screen === 'otp' ? 'forgot' : 'otp');
    };
    useEffect(
        /** Resets authentication feedback and fields when the screen changes. */
         () => {
        setMessage('');
        scroll.current?.scrollTo({ y: 0, animated: false });
        if (screen === 'signup') {
            setContact('');
            setPassword('');
            setPasswordRecovery(false);
        }
        if (screen === 'signin' && previous.current !== 'signin') {
            setContact('');
            setPassword('');
            setPasswordRecovery(false);
        }
        previous.current = screen;
    }, [screen, account.email]);
    useEffect(
        /** Registers the hardware back action and its cleanup. */
         () => {
        if (!recovery)
            return;
        /** Handles the device back action for this screen. */
        if (Platform.OS === 'web') return;
        const subscription = BackHandler.addEventListener('hardwareBackPress',  () => { back(); return true; });
        /** Removes the registered listeners when the effect ends. */
        return  () => subscription.remove();
    });
    useEffect(
        /** Synchronises component state with the effect dependencies. */
         () =>
        /** Clears the password-reset timer when the form closes. */
         () => {
        if (resetTimer.current)
            clearTimeout(resetTimer.current);
    }, []);
    useEffect(
        /** Updates the verification-code resend countdown. */
         () => {
        if (!resendSeconds)
            return;
        /** Decrements the remaining verification-code resend delay. */
        const timer = setTimeout( () => setResendSeconds(
            /** Builds the next resend seconds value from its previous state. */
             seconds => seconds - 1), 1000);
        /** Clears the pending timer when the effect ends. */
        return  () => clearTimeout(timer);
    }, [resendSeconds]);
    const startRecovery = async () => {
        if (submitting) return;
        const email = recoveryContact.trim().toLowerCase();
        if (!/^\S+@\S+\.\S+$/.test(email)) {setMessage('Enter a valid email address.'); return;}
        setSubmitting(true); setMessage('');
        try {
            const result = await apiRequest<{message:string}>('/api/auth/password-reset/request', {method:'POST', body:{email}});
            setPasswordRecovery(true); setOtp(['','','','','','']); setResendSeconds(60); setMessage(result.message); navigate('otp');
        } catch (error) {
            setMessage(error instanceof ApiError ? error.message : 'Could not reach the account service. Please try again.');
        } finally {setSubmitting(false);}
    };
    const submitReset = async () => {
        if (submitting) return;
        if (newPassword.length < 8 || newPassword.length > 128) {setMessage('Password must contain 8–128 characters.'); return;}
        if (newPassword !== confirmation) {setMessage('The passwords do not match.'); return;}
        setSubmitting(true); setMessage('');
        try {
            const result = await apiRequest<{message:string}>('/api/auth/password-reset/complete', {
                method:'POST', body:{email:recoveryContact.trim().toLowerCase(),verificationCode:otp.join(''),newPassword},
            });
            setNewPassword(''); setConfirmation(''); setOtp(['','','','','','']); setPasswordRecovery(false);
            onPasswordReset?.();
            setMessage(result.message);
        } catch (error) {
            setMessage(error instanceof ApiError ? error.message : 'Could not reset your password. Please try again.');
        } finally {setSubmitting(false);}
    };
    const signIn = async () => {
        if (submitting) return;
        const email = contact.trim().toLowerCase();
        if (!/^\S+@\S+\.\S+$/.test(email) || password.length < 8 || password.length > 128) {
            setMessage('Enter a valid email address and a password of 8–128 characters.');
            return;
        }
        setSubmitting(true);
        setMessage('');
        try {
            const response = await apiRequest<{token: string; user: {email: string}; profileCompleted: boolean}>('/api/auth/login', {
                method: 'POST', body: {email, password},
            });
            updateAccount({...account, ...(account.email.toLowerCase() === email ? {} : {
                name: '', firstName: undefined, lastName: undefined, qualification: undefined, fieldOfStudy: undefined,
                experienceLevel: undefined, interests: undefined, targetCareerID: undefined,
                targetRoles: [], industry: '', phone: '', location: '', workAreas: []
            }), email: response.user.email, password: '', token: response.token});
            setPassword('');
            onAuthenticated();
        } catch (error) {
            setMessage(error instanceof ApiError ? error.message : 'Could not reach the account service. Make sure the backend is running.');
        } finally { setSubmitting(false); }
    };
    const resendVerification = async () => {
        if (submitting || resendSeconds) return;
        const email = passwordRecovery ? recoveryContact.trim().toLowerCase() : account.email;
        if (!email) { setMessage('The registration email could not be found.'); return; }
        setSubmitting(true);
        setMessage('');
        try {
            const response = passwordRecovery
                ? await apiRequest<{message:string}>('/api/auth/password-reset/request', {method:'POST', body:{email}})
                : await apiRequest<{message:string}>('/api/auth/resend-verification', {method:'POST', body:{email}});
            setOtp(['', '', '', '', '', '']);
            setResendSeconds(60);
            setMessage(response.message);
            inputs.current[0]?.focus();
        } catch (error) {
            setMessage(error instanceof ApiError ? error.message : 'Could not reach the verification service.');
        } finally { setSubmitting(false); }
    };
    const verify = async () => {
        if (submitting) return;
        const verificationCode = otp.join("");

        if (verificationCode.length !== 6) {
            setMessage("Please enter the complete six-digit verification code.");
            return;
        }

        if (passwordRecovery) {
            setMessage('');
            navigate('reset');
            return;
        }

        if (!account.email) {
            setMessage("The registration email could not be found.");
            return;
        }

        try {
            setSubmitting(true);
            setMessage("");

            const response = await apiRequest<VerifyEmailResponse>(
                "/api/auth/verify-email",
                {
                    method: "POST",
                    body: {
                        email: account.email,
                        verificationCode,
                    },
                }
            );

            updateAccount({
                ...account,
                token: response.token,
            });

            onAuthenticated(true);
        } catch (error) {
            if (error instanceof ApiError) {
                setMessage(error.message);
            } else {
                setMessage(
                    "Could not verify the account. Make sure the backend is running."
                );
            }
        } finally {
            setSubmitting(false);
        }
    };

    const signUp = async () => {
        if (submitting) return;
        const email = contact.trim().toLowerCase();

        setMessage("");

        if (!agreed) {
            setMessage(
                "Please agree to the Terms & Conditions and Privacy Policy."
            );
            return;
        }

        if (!/^\S+@\S+\.\S+$/.test(email)) {
            setMessage("Please enter a valid email address.");
            return;
        }

        if (password.length < 8 || password.length > 128) {
            setMessage("Password must contain 8–128 characters.");
            return;
        }

        if (!industry.trim()) {
            setMessage("Please enter your industry.");
            return;
        }

        if (!targetRoles.length) {
            setMessage("Add at least one possible target role.");
            return;
        }

        try {
            setSubmitting(true);

            const response = await apiRequest<RegisterResponse>(
                "/api/auth/register",
                {
                    method: "POST",
                    body: {
                        email,
                        password,
                        name: name.trim(),
                        industry: industry.trim(),
                        phone: phone.trim(),
                        targetRoles,
                        workAreas,
                    },
                }
            );

            updateAccount({
                ...account,
                name: name.trim(),
                email: response.email,
                phone: phone.trim(),
                industry: industry.trim(),
                targetRoles,
                location,
                workAreas,
                password: "",
                token: undefined,
            });

            setPassword('');
            setOtp(['', '', '', '', '', '']);
            setResendSeconds(60);
            navigate("otp");
        } catch (error) {
            if (error instanceof ApiError) {
                setMessage(error.message);
            } else if (error instanceof TypeError) {
                setMessage(
                    "Could not reach the account service. Make sure the backend is running."
                );
            } else {
                setMessage("Something went wrong while creating the account.");
            }
        } finally {
            setSubmitting(false);
        }
    };
    /** Updates verification-code digits and moves focus between code fields. */
    const changeOtp = (value: string, index: number) => {
        const digits = value.replace(/\D/g, "").slice(0, 6);

        setMessage("");

        if (digits.length > 1) {
            const next = [...otp];

            digits
                .slice(0, 6 - index)
                .split("")
                .forEach((digit, offset) => {
                    next[index + offset] = digit;
                });

            setOtp(next);
            inputs.current[Math.min(index + digits.length, 5)]?.focus();
        } else {
            setOtp(old =>
                old.map((digit, i) => (i === index ? digits : digit))
            );

            if (digits && index < 5) {
                inputs.current[index + 1]?.focus();
            }
        }
    };
    /** Opens the requested legal document. */
    const policy =  (title: string) => {
        openLegal(title === 'Terms of Service' ? 'terms' : 'privacy');
    };
    const title = signin ? 'Sign In' : signup ? 'Sign Up' : screen === 'forgot' ? 'Forgot Password' : screen === 'otp' ? 'Enter OTP' : 'Reset Password';
    const minHeight = Math.max(650, height - insets.top - insets.bottom);
    return { industry,setIndustry,targetRoles,setTargetRoles,location, workAreas, setWorkAreas, phone, setPhone, screen, navigate, onAuthenticated, onPasswordReset, account, updateAccount, loadedAssets, assetError, height, width, compact, insets, signup, signin, recovery, passwordRecovery, name, setName, contact, setContact, password, setPassword, recoveryContact, setRecoveryContact, agreed, setAgreed, otp, setOtp, verified, setVerified, newPassword, setNewPassword, confirmation, setConfirmation, submitting, setSubmitting, message, setMessage, resendSeconds, setResendSeconds, inputs, scroll, resetTimer, previous, back, signIn, googleSignIn, signUp, resendVerification, startRecovery, verify, submitReset, changeOtp, policy, title, minHeight };
}
export type AuthModel = ReturnType<typeof useAuthModel>;
export type AuthFlowProps = Parameters<typeof useAuthModel>[0];
