/** Defines the shared application colours, typography and layout styles. */
import useResponsiveStyles from '../layout/useResponsiveStyles';
import { useTheme } from '../theme/ThemeContext';
import { StyleSheet } from 'react-native';
const s = StyleSheet.create({
    flex: { flex: 1 }, safe: { flex: 1, backgroundColor: '#fff' }, shell: { flex: 1, width: '100%', maxWidth: 720, alignSelf: 'center' },
    content: { paddingTop: 48, paddingBottom: 110, flexGrow: 1 }, title: { fontSize: 30, lineHeight: 36, fontWeight: '600', letterSpacing: -0.8 },
    muted: { color: '#71717a', fontSize: 14, lineHeight: 18.9, marginTop: 6 }, header: { minHeight: 108, flexDirection: 'row', alignItems: 'flex-start', justifyContent: 'space-between', gap: 20 },
    titleRow: { flexDirection: 'row', alignItems: 'flex-start', gap: 14 }, back: { fontSize: 25, lineHeight: 25, paddingTop: 2 }, menu: { minWidth: 44, minHeight: 44, width: 28, height: 28, padding: 5, gap: 4, justifyContent: 'center' }, menuLine: { height: 2, width: 18, backgroundColor: '#111' },
    heading: { fontSize: 16, fontWeight: '700', marginTop: 28, marginBottom: 16 }, bold: { fontSize: 14, fontWeight: '700', color: '#1f2024' }, small: { fontSize: 12, color: '#71727a', marginTop: 5 },
    button: { width: '100%', minHeight: 49, backgroundColor: '#000', borderRadius: 5, justifyContent: 'center', alignItems: 'center', marginTop: 24 }, buttonText: { fontSize: 13, color: '#fff' },
    outline: { width: '100%', minHeight: 45, borderWidth: 1, borderColor: '#09090b', borderRadius: 6, paddingHorizontal: 30, backgroundColor: '#fff', marginTop: 28, alignSelf: 'center', minWidth: 0 }, dark: { color: '#09090b', fontSize: 14 },
    card: { minHeight: 69, padding: 16, marginBottom: 18, borderRadius: 16, backgroundColor: '#f8f9fe', flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between', gap: 8 }, cardArrow: { color: '#8f9098', fontSize: 25 },
    stepper: { flexDirection: 'row', gap: 4, marginTop: 4 }, step: { flex: 1, height: 4, borderRadius: 99, backgroundColor: '#c1c9d2' }, stepLabel: { fontSize: 14, marginTop: 14, marginBottom: 4 }, upload: { borderRadius: 16, backgroundColor: '#f8f9fe', paddingBottom: 24, overflow: 'hidden', alignItems: 'center' }, uploadVisual: { height: 120, width: '100%', backgroundColor: '#eaf2ff', alignItems: 'center', justifyContent: 'center', marginBottom: 17 }, uploadCopy: { marginTop: 4, color: '#71727a', fontSize: 12 }, uploadHint: { marginTop: 20, color: '#494a50', fontSize: 12 }, chooseFile: { width: 110, padding: 9, marginTop: 28, backgroundColor: '#000', borderRadius: 4 }, chooseFileText: { color: '#fff', fontSize: 12, textAlign: 'center' },
    hero: { minHeight: 170, borderWidth: 1, borderColor: '#e4e4e7', borderRadius: 6, backgroundColor: '#f8f9fe', padding: 25, marginTop: 14, marginBottom: 32 }, heroTitle: { fontSize: 24, fontWeight: '700', marginBottom: 5 }, heroCopy: { color: '#71717a', fontSize: 16, marginVertical: 16 }, heroLink: { marginTop: 22, fontSize: 16, fontWeight: '600' },
    roleSelect: { height: 48, borderWidth: 1, borderColor: '#c5c6cc', borderRadius: 12, backgroundColor: '#f8f9fe', paddingHorizontal: 16, flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between' }, roleText: { flexShrink: 1, color: '#8f9098', fontSize: 16 }, score: { minHeight: 150, justifyContent: 'center', alignItems: 'center' }, scoreText: { fontSize: 24, fontWeight: '700', color: '#1bc457' }, scoreCaption: { color: '#1bc457', fontSize: 14, marginTop: 2 }, feedback: { minHeight: 70, flexDirection: 'row', gap: 18, alignItems: 'center', paddingVertical: 12, paddingHorizontal: 16, borderBottomWidth: 1, borderColor: '#e5e7eb' }, feedbackText: { color: '#494a50', fontSize: 13, flex: 1 },
    tabs: { flexDirection: 'row', padding: 4, marginTop: 65, marginBottom: 42, borderRadius: 16, backgroundColor: '#f8f9fe' }, tab: { flex: 1, height: 34, justifyContent: 'center', alignItems: 'center', borderRadius: 12 }, tabText: { fontSize: 12, fontWeight: '700', color: '#71727a' }, selectedTab: { backgroundColor: '#fff', boxShadow: '0px 0px 5px #edf0f8' }, skill: { minHeight: 78, paddingVertical: 16, paddingHorizontal: 8, borderBottomWidth: 1, borderColor: '#eceef2', flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between', gap: 8 }, skillSummary: { flex: 1, flexWrap: 'wrap', flexDirection: 'row', gap: 8, alignItems: 'center' }, skillScore: { fontSize: 12, color: '#71727a' }, badge: { paddingVertical: 5, paddingHorizontal: 10, borderRadius: 13, fontSize: 10, fontWeight: '600' }, have: { color: '#0a3622', backgroundColor: '#d1e7dd' }, partial: { color: '#664d03', backgroundColor: '#fff3cd' }, missing: { color: '#58151c', backgroundColor: '#f8d7da' },
    search: { height: 45, borderRadius: 24, paddingHorizontal: 16, flexDirection: 'row', gap: 15, alignItems: 'center', backgroundColor: '#f8f9fe' }, searchInput: { flex: 1, fontFamily: 'Inter', fontSize: 14, color: '#1f2024', padding: 0 }, chips: { flexWrap: 'wrap', flexDirection: 'row', gap: 14, marginTop: 22, marginBottom: 20, justifyContent: 'flex-end' }, chip: { height: 37, paddingHorizontal: 15, borderWidth: 1, borderColor: '#d4d6dd', borderRadius: 8, justifyContent: 'center' }, chipText: { fontSize: 13 }, filter: { height: 37, paddingHorizontal: 15, borderWidth: 1, borderColor: '#d4d6dd', borderRadius: 12, alignSelf: 'flex-start', justifyContent: 'center' }, jobResults: { marginTop: 28 },
    match: { paddingVertical: 16, paddingHorizontal: 30, marginTop: 8, marginBottom: 25, borderRadius: 16, backgroundColor: 'rgba(52,199,89,0.09)' }, matchText: { color: '#20b954', fontSize: 14, fontWeight: '500' }, description: { minHeight: 145, padding: 18, borderWidth: 1, borderColor: '#e4e4e7', borderRadius: 6, backgroundColor: '#f8f9fe' }, descriptionText: { color: '#71727a', fontSize: 13, lineHeight: 20.8 }, tags: { flexDirection: 'row', flexWrap: 'wrap', gap: 16, marginTop: 64, marginHorizontal: 16, marginBottom: 28 },
    resource: { minHeight: 72, padding: 16, marginBottom: 14, borderRadius: 16, backgroundColor: '#f8f9fe', flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', gap: 12 }, resourceSmall: { marginTop: 4, color: '#71727a', fontSize: 12 }, checkbox: { width: 18, height: 18, borderWidth: 1, borderColor: '#767676', borderRadius: 3, alignItems: 'center', justifyContent: 'center' }, checked: { backgroundColor: '#20bd59', borderColor: '#20bd59' }, checkmark: { color: '#fff', fontSize: 12 },
    center: { textAlign: 'center' }, successMark: { width: 55, height: 55, alignSelf: 'center', marginBottom: 28, borderWidth: 2, borderColor: '#20bd59', borderRadius: 28, alignItems: 'center', justifyContent: 'center' }, successText: { color: '#20bd59', fontSize: 24 }, completeCopy: { textAlign: 'center', color: '#71717a', fontSize: 14, marginTop: 14, marginBottom: 34 },
    profile: { flexDirection: 'row', alignItems: 'center', gap: 22, marginHorizontal: 8, marginBottom: 36 }, avatar: { width: 48, height: 48, borderRadius: 24 }, profileName: { fontSize: 16, fontWeight: '700' }, profileTarget: { flexShrink: 1, marginTop: 4, color: '#71717a', fontSize: 12 }, settingsGroup: { marginHorizontal: 18, marginBottom: 25 }, settingsHeading: { fontSize: 14, fontWeight: '700', marginBottom: 10 }, setting: { minHeight: 44, paddingHorizontal: 7, flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between' }, settingText: { flexShrink: 1, color: '#2f3036', fontSize: 16 }, settingArrow: { color: '#8f9098', fontSize: 23 }, logout: { marginVertical: 5, marginHorizontal: 25 }, logoutText: { color: '#db2777', fontSize: 16 },
    nav: { position: 'absolute', bottom: 0, left: 0, right: 0, minHeight: 68, paddingVertical: 12, paddingHorizontal: 22, flexDirection: 'row', justifyContent: 'space-around', backgroundColor: 'rgba(255,255,255,0.96)', borderTopWidth: 1, borderColor: '#f5f5f5' }, navItem: { minWidth: 44, minHeight: 44, width: 42, height: 42, padding: 9 }, error: { color: '#bd2534', backgroundColor: '#fff1f2', padding: 14, borderRadius: 8, marginBottom: 20 },
});
export default s;

const darkColors: Record<string, string> = {
    '#fff':'#151c23', '#f8f9fe':'#202a34', '#eaf2ff':'#253745',
    '#09090b':'#f4f1ea', '#1f2024':'#f4f1ea', '#2f3036':'#f4f1ea', '#111':'#f4f1ea',
    '#71717a':'#aab3be', '#71727a':'#aab3be', '#494a50':'#b6bec8',
    '#e4e4e7':'#394552', '#e5e7eb':'#394552', '#eceef2':'#394552',
};
const darkStyles = Object.fromEntries(Object.entries(s).map(
    /** Transforms each item into the required display or state value. */
     ([name, style]) => [name,
    Object.fromEntries(Object.entries(style).map(
        /** Transforms each item into the required display or state value. */
         ([key,value]) => [key,
        typeof value === 'string' && (key === 'backgroundColor' || key === 'color' || key === 'borderColor')
            ? (name === 'buttonText' || name === 'chooseFileText' || name === 'checkmark' ? value : darkColors[value] || value)
            : value,
    ])),
])) as typeof s;
/** Returns shared application styles for the current theme. */
export function useAppStyles() {
    const { dark } = useTheme();
    return useResponsiveStyles(dark ? darkStyles : s);
}
