/** Defines the colours and responsive styles used by authentication screens. */
import useResponsiveStyles from '../layout/useResponsiveStyles';
import { StyleSheet, Platform } from 'react-native';
export const colors = {
    primary: '#D4AF53', title: '#E8B866', text: '#3B4054', muted: '#61677D',
    placeholder: '#7C8BA0', field: '#F5F9FE', border: '#D4AF53', pale: 'rgba(232,184,102,0.6)',
};
const styles = StyleSheet.create({
    flex: { flex: 1 }, safe: { flex: 1, backgroundColor: '#F4F1EA' },
    canvas: { flexGrow: 1, width: '100%', alignSelf: 'stretch', paddingBottom: 100 },
    body: { paddingHorizontal: 31.22 },
    statusBar: { height: 42, width: '100%' }, time: { position: 'absolute', left: 24, top: 12 },
    statusIcons: { position: 'absolute', right: 24, top: 12, flexDirection: 'row', gap: 3, alignItems: 'flex-start' },
    back: { marginTop: 32, width: 24, height: 24 },
    header: { marginTop: 80, alignItems: 'center', gap: 16 },
    title: { fontFamily: 'PoppinsSemiBold', fontSize: 32, lineHeight: 40, color: colors.title, textAlign: 'center', includeFontPadding: false },
    description: { fontFamily: 'Poppins', fontSize: 14, lineHeight: 22, color: colors.muted, textAlign: 'center', includeFontPadding: false },
    socialRow: { marginTop: 24, flexDirection: 'row', gap: 21 },
    social: { flex: 1, height: 56, padding: 16, borderRadius: 14, backgroundColor: colors.field, flexDirection: 'row', alignItems: 'center', justifyContent: 'center', gap: 16 },
    socialIcon: { width: 24, height: 24, overflow: 'hidden' },
    socialText: { width: 91, fontFamily: 'PoppinsMedium', fontSize: 16, lineHeight: 24, color: colors.muted, includeFontPadding: false },
    divider: { marginVertical: 16, height: 22, flexDirection: 'row', gap: 11, alignItems: 'center' },
    dividerClip: { flex: 1, height: 1, overflow: 'hidden', transform: [{ translateY: 2.5 }] },
    or: { fontFamily: 'Poppins', fontSize: 14, lineHeight: 22, color: '#a6a6a4', width: 17, textAlign: 'center', includeFontPadding: false },
    fields: { gap: 16 },
    field: { height: 60, paddingHorizontal: 23, borderWidth: 1, borderColor: 'transparent', borderRadius: 14, backgroundColor: colors.field, flexDirection: 'row', alignItems: 'center', gap: 16 },
    focused: { borderColor: colors.primary },
    input: { flex: 1, minWidth: 0, fontFamily: 'Poppins', fontSize: 16, lineHeight: 24, padding: 0, borderWidth: 0, backgroundColor: 'transparent', color: '#262626', includeFontPadding: false, ...(Platform.OS === 'web' ? { outlineWidth: 0, outlineStyle: 'solid' as const, outlineColor: 'transparent' } : {}) },
    forgot: { marginTop: 8, alignSelf: 'flex-end', marginRight: 14 },
    forgotText: { fontFamily: 'Poppins', fontSize: 12, lineHeight: 22, color: '#9B6E2F', includeFontPadding: false },
    termsRow: { marginTop: 12, flexDirection: 'row', gap: 14, alignItems: 'flex-start' },
    checkbox: { width: 24, height: 24, borderRadius: 5, backgroundColor: colors.field, alignItems: 'center', justifyContent: 'center' },
    checked: { backgroundColor: colors.primary }, checkmark: { color: '#fff', fontSize: 16 },
    terms: { flex: 1, fontFamily: 'Poppins', fontSize: 12, lineHeight: 22, color: '#a6a6a4', includeFontPadding: false },
    link: { color: '#9B6E2F' },
    primary: { minHeight: 60, borderRadius: 14, backgroundColor: colors.primary, flexDirection: 'row', alignItems: 'center', justifyContent: 'center', gap: 12, boxShadow: '0px 6px 12px rgba(0,0,0,0.38), 0px 12px 22px -8px rgba(184,118,42,0.3)' },
    primaryText: { fontFamily: 'PoppinsMedium', fontSize: 16, lineHeight: 24, color: '#fff', includeFontPadding: false },
    spinner: { width: 24, height: 24, alignItems: 'center', justifyContent: 'center' },
    accountSwitch: { marginTop: 16, marginLeft: 16, fontFamily: 'Poppins', fontSize: 14, lineHeight: 22, color: colors.text, includeFontPadding: false },
    recoveryFields: { marginTop: 24 },
    otpRow: { marginTop: 32, flexDirection: 'row', gap: 16 },
    otpInput: { flex: 1, minWidth: 0, height: 70, borderWidth: 1, borderColor: colors.border, borderRadius: 12, backgroundColor: colors.field, color: colors.text, fontFamily: 'PoppinsMedium', fontSize: 24, textAlign: 'center', padding: 0, includeFontPadding: false, ...(Platform.OS === 'web' ? { outlineWidth: 0, outlineStyle: 'solid' as const, outlineColor: 'transparent' } : {}) },
    progress: { position: 'absolute', bottom: 58, alignSelf: 'center', flexDirection: 'row', gap: 12 },
    progressSegment: { width: 32, height: 4, borderRadius: 2, backgroundColor: colors.pale }, progressActive: { backgroundColor: colors.primary },
    message: { marginTop: 20, color: colors.muted, fontFamily: 'Poppins', fontSize: 12, lineHeight: 22 },
});
export default styles;

/** Returns responsive styles for authentication screens. */
export function useAuthStyles() { return useResponsiveStyles(styles); }
