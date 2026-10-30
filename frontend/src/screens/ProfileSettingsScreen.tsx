/** Edits the profile fields persisted by the authenticated backend. */
import ScreenHeader, {screenHeaderTop} from '../components/ScreenHeader';
import {apiRequest, ApiError} from '../api/client';
import {saveProfile} from '../api/profile';
import CareerPreferencesFields from '../components/CareerPreferencesFields';
import WorkAreaPicker from '../location/WorkAreaPicker';
import type {WorkArea} from '../location/types';
import {useEffect, useState} from 'react';
import {KeyboardAvoidingView, Platform, Pressable, ScrollView, StyleSheet, TextInput, View} from 'react-native';
import {useSafeAreaInsets} from 'react-native-safe-area-context';
import Text from '../components/Text';
import type {AppModel} from '../navigation/useAppModel';

type Career = {careerID: string; careerName: string};
type Suggestions = {hasCvAnalysis:boolean; hasTranscriptAnalysis:boolean; suggestions:{careerID:string;careerName:string;matchedSkills:{skillName:string;fromCv:boolean;fromTranscript:boolean}[]}[]};

export default function ProfileSettingsScreen({model}: {model: AppModel}) {
    const insets = useSafeAreaInsets();
    const inferredNames = model.account.name.trim().split(/\s+/);
    const [firstName, setFirstName] = useState(model.account.firstName || inferredNames[0] || '');
    const [lastName, setLastName] = useState(model.account.lastName || inferredNames.slice(1).join(' '));
    const [qualification, setQualification] = useState(model.account.qualification || '');
    const [fieldOfStudy, setFieldOfStudy] = useState(model.account.fieldOfStudy || '');
    const [experienceLevel, setExperienceLevel] = useState(model.account.experienceLevel || '');
    const [interests, setInterests] = useState(model.account.interests || '');
    const [targetCareerID, setTargetCareerID] = useState(model.account.targetCareerID || '');
    const [industry, setIndustry] = useState(model.account.industry || '');
    const [targetRoles, setTargetRoles] = useState<string[]>(model.account.targetRoles ?? []);
    const [workAreas, setWorkAreas] = useState<WorkArea[]>(model.account.workAreas ?? []);
    const [careers, setCareers] = useState<Career[]>([]);
    const [suggestions, setSuggestions] = useState<Suggestions | null>(null);
    const [notice, setNotice] = useState('');
    const [saving, setSaving] = useState(false);
    const [focus, setFocus] = useState('');

    useEffect(() => {
        const controller = new AbortController();
        apiRequest<Career[]>('/api/careers', {signal: controller.signal}).then(setCareers).catch(() => {});
        return () => controller.abort();
    }, []);

    useEffect(() => {
        if (!model.account.token) return;
        const controller = new AbortController();
        apiRequest<Suggestions>('/api/careers/suggestions', {token:model.account.token, signal:controller.signal}).then(setSuggestions).catch(() => {});
        return () => controller.abort();
    }, [model.account.token]);

    const save = async () => {
        if (!model.account.token || saving) return;
        if (![firstName, lastName, qualification, fieldOfStudy].every(value => value.trim())) {
            setNotice('First name, last name, qualification and field of study are required.');
            return;
        }
        setSaving(true); setNotice('');
        // Keep the chosen career first in targetRoles so the dashboard's role sync does not switch it back.
        const chosen = careers.find(item => item.careerID === targetCareerID);
        const existingRoles = targetRoles;
        const roles = chosen
            ? [chosen.careerName, ...existingRoles.filter(role => role.trim().toLowerCase() !== chosen.careerName.trim().toLowerCase())]
            : existingRoles;
        try {
            const profile = await saveProfile(model.account.token, {
                fName: firstName.trim(), lName: lastName.trim(), qualification: qualification.trim(),
                fieldOfStudy: fieldOfStudy.trim(), experienceLevel: experienceLevel.trim(),
                interests: interests.trim(), targetCareerID: targetCareerID || null,
                industry: industry.trim(), phone: model.account.phone ?? '',
                targetRoles: roles, workAreas,
            });
            model.setAccount(account => ({...account, name: `${profile.fName} ${profile.lName}`.trim(),
                firstName: profile.fName, lastName: profile.lName, qualification: profile.qualification,
                fieldOfStudy: profile.fieldOfStudy, experienceLevel: profile.experienceLevel ?? '',
                interests: profile.interests ?? '', targetCareerID: profile.targetCareerID ?? undefined,
                industry: profile.industry ?? '', phone: profile.phone ?? '',
                targetRoles: profile.targetRoles ?? roles,
                workAreas: profile.workAreas ?? []}));
            setTargetRoles(profile.targetRoles ?? roles);
            model.setRole(0);
            model.setCareerAssessment(null);
            setNotice('Profile saved to your account.');
        } catch (error) {
            setNotice(error instanceof ApiError ? error.message : 'Could not save your profile. Please try again.');
        } finally {setSaving(false);}
    };

    const fields = [
        ['firstName', 'First name', firstName, setFirstName, 50], ['lastName', 'Last name', lastName, setLastName, 50],
        ['qualification', 'Qualification', qualification, setQualification, 100], ['fieldOfStudy', 'Field of study', fieldOfStudy, setFieldOfStudy, 100],
        ['experienceLevel', 'Experience level', experienceLevel, setExperienceLevel, 50], ['interests', 'Interests', interests, setInterests, 1000],
    ] as const;

    return <KeyboardAvoidingView style={s.safe} behavior={Platform.OS === 'ios' ? 'padding' : undefined}>
        <ScrollView contentContainerStyle={[s.content, {paddingTop: screenHeaderTop(Math.min(model.width, 480) / 408.393, insets.top), paddingBottom: insets.bottom + 24}]} keyboardShouldPersistTaps="handled">
            <ScreenHeader title="PROFILE SETTINGS" onBack={() => model.goBack('settings')} backLabel="Back to settings" scale={Math.min(model.width, 480) / 408.393}/>
            <Text style={s.copy}>These details are saved securely to your LevelUp profile.</Text>
            {fields.map(([id, label, value, change, maxLength]) => <View key={id} style={s.field}><Text style={s.label}>{label}</Text><TextInput
                accessibilityLabel={label} value={value} onChangeText={change} placeholder={label} placeholderTextColor="#696967"
                onFocus={() => setFocus(id)} onBlur={() => setFocus('')} maxLength={maxLength}
                multiline={id === 'interests'} style={[s.input, id === 'interests' && s.multiline, focus === id && s.focused]}/>
                {value.length >= maxLength * 0.8 && <Text accessibilityLiveRegion="polite" style={[s.counter, value.length >= maxLength && s.counterLimit]}>{value.length >= maxLength ? `Maximum of ${maxLength} characters reached` : `${value.length}/${maxLength} characters`}</Text>}</View>)}
            <View style={s.field}><Text style={s.label}>Career preferences</Text>
                <CareerPreferencesFields industry={industry} onIndustryChange={setIndustry} targetRoles={targetRoles} onRolesChange={setTargetRoles}/>
                <WorkAreaPicker value={workAreas} onChange={setWorkAreas}/>
            </View>
            <View style={s.field}><Text style={s.label}>Suggested careers from your documents</Text>
                <Text style={s.copy}>{suggestions?.suggestions.length ? 'Suggestions are based on skills matched to our career catalogue. Choose a career and save your profile.' : 'Analyse your CV and transcript to find career suggestions supported by their detected skills.'}</Text>
                {suggestions?.suggestions.map(career => <Pressable key={career.careerID} accessibilityRole="radio" accessibilityState={{selected:targetCareerID === career.careerID}} onPress={() => setTargetCareerID(career.careerID)} style={[s.option, targetCareerID === career.careerID && s.selected]}><Text style={s.label}>{career.careerName}</Text><Text style={s.copy}>{career.matchedSkills.map(skill => `${skill.skillName} (${skill.fromCv && skill.fromTranscript ? 'CV and transcript' : skill.fromCv ? 'CV' : 'transcript'})`).join(', ')}</Text></Pressable>)}
            </View>
            <View style={s.field}><Text style={s.label}>Target career</Text>{careers.map(career => <Pressable key={career.careerID}
                accessibilityRole="radio" accessibilityState={{selected: targetCareerID === career.careerID}}
                onPress={() => setTargetCareerID(career.careerID)} style={[s.option, targetCareerID === career.careerID && s.selected]}>
                <Text style={s.label}>{targetCareerID === career.careerID ? '✓ ' : ''}{career.careerName}</Text>
            </Pressable>)}</View>
            <Text style={s.email}>Account email: {model.account.email}</Text>
            {!!notice && <Text accessibilityLiveRegion="polite" style={s.notice}>{notice}</Text>}
            <Pressable accessibilityRole="button" accessibilityLabel="Save profile" accessibilityState={{disabled: saving, busy: saving}}
                disabled={saving} onPress={save} style={[s.save, saving && s.disabled]}><Text style={s.saveText}>{saving ? 'Saving…' : 'Save profile'}</Text></Pressable>
        </ScrollView>
    </KeyboardAvoidingView>;
}

const s = StyleSheet.create({
    safe:{flex:1,backgroundColor:'#000'}, content:{width:'100%',maxWidth:480,alignSelf:'center',paddingHorizontal:24,flexGrow:1},
    copy:{fontSize:14,lineHeight:22,color:'#a6a6a4',marginBottom:20}, field:{marginBottom:18}, label:{color:'#fff',fontSize:14,marginBottom:8},
    input:{minHeight:52,borderWidth:1,borderColor:'#353e44',borderRadius:10,padding:14,backgroundColor:'#060f14',color:'#fff',fontSize:16},
    multiline:{minHeight:90,textAlignVertical:'top'}, focused:{borderColor:'#d4af53'}, option:{minHeight:48,borderWidth:1,borderColor:'#353e44',borderRadius:10,padding:14,marginTop:8,backgroundColor:'#060f14'},
    selected:{borderColor:'#d4af53'}, email:{color:'#a6a6a4',fontSize:14,marginBottom:18}, notice:{color:'#e8b866',fontSize:14,lineHeight:22,marginBottom:16},
    save:{minHeight:50,backgroundColor:'#d4af53',borderRadius:10,alignItems:'center',justifyContent:'center'}, disabled:{opacity:.6}, saveText:{color:'#000',fontSize:16,fontWeight:'700'},
    counter:{color:'#a6a6a4',fontSize:12,marginTop:6}, counterLimit:{color:'#e8b866'},
});
