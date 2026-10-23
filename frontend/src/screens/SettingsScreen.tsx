import type {BundledAsset} from '../platform/assets';
/** Displays grouped account, library, preference and privacy controls. */
import ScreenHeader, {screenHeaderTop} from '../components/ScreenHeader';
import OpportunityNotifications from '../notifications/OpportunityNotifications';
import { useState } from 'react';
import { View, Pressable, Image, ScrollView, StyleSheet, Modal, useWindowDimensions } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import Svg, { Path } from 'react-native-svg';
import Text from '../components/Text';
import ChevronLeft from '../components/ChevronLeft';
import BottomNavigation from '../navigation/BottomNavigation';
import LocalVector from '../onboarding/LocalVector';
import useResponsiveStyles from '../layout/useResponsiveStyles';
import type { AppModel } from '../navigation/useAppModel';

/** Displays grouped account, library, preference and privacy controls. */
export default function SettingsScreen({ model }: { model: AppModel }) {
    const responsive = useResponsiveStyles(base);
    const insets = useSafeAreaInsets();
    const scale = Math.min(model.width,480) / 408.393;
    const { height } = useWindowDimensions();
    const compact = (height-insets.top-insets.bottom)/scale < 760;
    const [viewportHeight,setViewportHeight] = useState(0);
    const [contentHeight,setContentHeight] = useState(0);
    const s = {
        ...responsive,

        profile:{...responsive.profile,minHeight:(compact ? 90 : 100)*scale,marginBottom:(compact ? 8 : 12)*scale},
        profileCopy:{...responsive.profileCopy,paddingVertical:10*scale},
        card:{...responsive.card,marginBottom:(compact ? 8 : 12)*scale},
        copy:{...responsive.copy,marginTop:6*scale},
        group:{...responsive.group,marginBottom:(compact ? 8 : 12)*scale},
    };
    const [panel, setPanel] = useState('');
    const [consentAgreed,setConsentAgreed]=useState(false);
    const [notice, setNotice] = useState('');
    const [notificationsOpen,setNotificationsOpen]=useState(false);
    const notifications=model.opportunityNotifications;
    const icons: Record<string, BundledAsset> = {
        'Profile settings': {uri:'/assets/settings/edit-profile.png'},
        'Saved Opportunities': {uri:'/assets/settings/saved-opportunities.png'},
        'Saved Resources': {uri:'/assets/settings/saved-resources.png'},
        Language: {uri:'/assets/settings/language.png'},
        Notifications: {uri:'/assets/settings/notifications.png'},
        'Privacy & Data Settings': {uri:'/assets/settings/privacy.png'},
    };
    /** Displays a settings action with its title, description and optional icon. */
    const card =  (title: string, description: string, onPress: () => void) => <Pressable key={title} accessibilityRole="button" accessibilityLabel={title} accessibilityHint={description} onPress={onPress} style={[s.card,!icons[title]&&{paddingLeft:14*scale}]}>{icons[title] && <Image source={icons[title]} style={[s.cardIcon,title === 'Saved Resources' && {width:41*scale,height:41*scale,left:5*scale},title === 'Language' && {width:44*scale,height:44*scale},title === 'Privacy & Data Settings' && {opacity:.91}]} resizeMode="contain"/>}<Text style={s.cardTitle}>{title}</Text><Text style={s.copy}>{description}</Text></Pressable>;
    return <View style={s.safe}><View style={s.canvas}>
        <View pointerEvents="none" style={StyleSheet.absoluteFill}>
            <View testID="settings-upper-contours" style={{ position:'absolute', top:-14 * scale, opacity:.21, transformOrigin:'top left', transform:[{scale}] }}><LocalVector source={{uri:'/assets/settings/upper-contours.svg'}} width={414.8} height={186.551}/></View>
            <View testID="settings-lower-contours" style={{ position:'absolute', bottom:0, opacity:.21, transformOrigin:'bottom left', transform:[{scale}] }}><LocalVector source={{uri:'/assets/settings/lower-contours.svg'}} width={414.8} height={310.065}/></View>
        </View>
        <ScrollView testID="settings-content" style={{flex:1}} onLayout={
            /** Records the measured layout dimensions for this view. */
             event => setViewportHeight(event.nativeEvent.layout.height)} onContentSizeChange={
            /** Records the measured layout dimensions for this view. */
             (_,value) => setContentHeight(value)} scrollEnabled={contentHeight > viewportHeight + 1} showsVerticalScrollIndicator={false} alwaysBounceVertical={false} contentContainerStyle={[s.content,{paddingTop:screenHeaderTop(scale,insets.top),paddingBottom:4*scale}]}>
            <ScreenHeader title="SETTINGS" onBack={
                /** Opens the home screen. */
                 ()=>model.goBack('home')} backLabel="Back to previous page" scale={scale} style={{marginBottom:17*scale}}/>
            <View testID="settings-profile-summary" style={s.profile}><View style={s.profileCopy}><Text style={s.cardTitle}>{model.account.name || 'Your profile'}</Text><Text style={s.student}>{model.account.industry || 'Career profile'}</Text></View></View>
            <Text testID="settings-section-heading" style={s.group}>Account</Text>
            {card('Profile settings','Personal details and account security',
                /** Opens the profile settings screen. */
                 () => model.go('profileSettings'))}
            <Text testID="settings-section-heading" style={s.group}>Your Library</Text>
            {card('Saved Opportunities',`${model.opportunityModel.saved.length} saved jobs and programmes`,
                /** Opens the jobs screen. */
                 () => { model.opportunityModel.reset(); model.opportunityModel.setSavedOnly(true); model.go('jobs'); })}
            {card('Saved Resources',`${model.savedTopics.length} learning resources`,
                /** Opens the Saved Resources panel. */
                 () => {setPanel('Saved Resources');setNotice('');})}
            <Text testID="settings-section-heading" style={s.group}>Preferences</Text>
            {card('Language','English (South Africa)',
                /** Opens the Language panel. */
                 () => { setPanel('Language'); setNotice('English (South Africa) is the available language.'); })}
            {card('Notifications','Job alerts and reminders',
                /** Updates whether the notification inbox is visible. */
                 () => setNotificationsOpen(true))}
            <Text testID="settings-section-heading" style={s.group}>Privacy & Support</Text>
            {card('Privacy & Data Settings','Control how your data is used',
                /** Opens the Privacy & Data Settings panel. */
                 () => { setPanel('Privacy & Data Settings'); setNotice(''); })}
            <Pressable accessibilityRole="button" accessibilityLabel="Log out" style={s.logout} onPress={model.signOut}><Svg width={24} height={24} viewBox="0 0 24 24" fill="none" stroke="rgba(225,19,19,0.7)" strokeWidth={2} strokeLinecap="round" strokeLinejoin="round"><Path d="M9 3H4V21H9M10 12H22M17 7L22 12L17 17"/></Svg><Text style={s.logoutText}>Log Out</Text></Pressable>
        </ScrollView>
        <BottomNavigation screen={model.screen} go={model.go} inline/>
        <OpportunityNotifications visible={notificationsOpen} onClose={
            /** Updates whether the notification inbox is visible. */
             ()=>setNotificationsOpen(false)} alerts={notifications.alerts} unread={notifications.unread} onMarkAll={notifications.markAllRead} availableIds={model.opportunityModel.allOpportunities.map(
            /** Transforms each opportunitie into the required display or state value. */
             item=>item.id)} onOpen={
            /** Opens the job screen. */
             alert=>{
            notifications.markRead(alert.id);
            /** Finds the opportunity referenced by the selected notification. */
            const item=model.opportunityModel.allOpportunities.find( opportunity=>opportunity.id===alert.opportunityId);
            setNotificationsOpen(false);
            if(item){model.opportunityModel.setSelected(item);model.setJob(item.title);model.go('job');}
            else{setPanel('Notifications');setNotice('This opportunity is currently unavailable.');}
        }}/>
        <Modal visible={!!panel} transparent animationType="fade" onRequestClose={
            /** Closes the selected dialog or detail view. */
             () => setPanel('')}><View style={s.scrim}><View style={[s.panel,{maxHeight:'90%',padding:0}]}><ScrollView contentContainerStyle={{padding:20}} keyboardShouldPersistTaps="handled"><Text accessibilityRole="header" style={s.panelTitle}>{panel}</Text>{!!notice && <Text style={s.panelCopy}>{notice}</Text>}{panel === 'Saved Resources' && <ScrollView style={{maxHeight:Math.min(360,height*.45)}} contentContainerStyle={{gap:10}}>{!model.savedTopics.length&&<Text style={s.panelCopy}>No saved learning resources yet. Use the bookmark in Mission Intel to save a topic.</Text>}{model.savedTopics.map(
            /** Renders a topic entry from its data. */
             topic=><Pressable key={topic} accessibilityRole="button" accessibilityLabel={'Open saved resource: '+topic} onPress={
            /** Opens the topic screen. */
             ()=>{setPanel('');model.setTopic(topic);model.go('topic');}} style={s.resourceItem}><Text style={s.cardTitle}>{topic}</Text></Pressable>)}</ScrollView>}{panel === 'Privacy & Data Settings'  && <>{card('Privacy Policy','Learn how your information is used',
            /** Closes the selected dialog or detail view. */
             () => { setPanel(''); model.openLegal('privacy'); })}{card('Terms & Conditions','Read the terms of service',
            /** Closes the selected dialog or detail view. */
             () => { setPanel(''); model.openLegal('terms'); })}<Text style={s.panelCopy}>Consent: {model.consent.consent?.status==='granted'?'Agreed':model.consent.consent?.status==='withdrawn'?'Withdrawn':'Not given'}</Text>{card('Manage consent','Review or withdraw your agreement',
            /** Opens the Manage consent panel. */
             ()=>{setConsentAgreed(false);setPanel('Manage consent');})}</>}{panel==='Manage consent'&&<><Text style={s.panelCopy}>Withdrawing consent pauses document analysis and personalisation on this device. It does not delete your account or files.</Text><View style={{flexDirection:'row',gap:12,alignItems:'center',marginBottom:16}}><Pressable accessibilityRole="checkbox" accessibilityLabel="Agree to terms and privacy policy" accessibilityState={{checked:consentAgreed}} aria-checked={consentAgreed} onPress={
            /** Applies the selected toggle or checkbox value. */
             ()=>setConsentAgreed(!consentAgreed)} style={{minWidth:44,minHeight:44,borderRadius:8,borderWidth:1,borderColor:'#d4af53',alignItems:'center',justifyContent:'center'}}><Text style={s.cardTitle}>{consentAgreed?'✓':''}</Text></Pressable><View style={{flex:1}}><Text style={s.panelCopy}>I agree to the Terms &amp; Conditions and Privacy Policy.</Text><Pressable accessibilityRole="link" onPress={
            /** Closes the selected dialog or detail view. */
             ()=>{setPanel('');model.openLegal('terms');}}><Text style={s.cardTitle}>Read Terms &amp; Conditions</Text></Pressable><Pressable accessibilityRole="link" onPress={
            /** Closes the selected dialog or detail view. */
             ()=>{setPanel('');model.openLegal('privacy');}}><Text style={s.cardTitle}>Read Privacy Policy</Text></Pressable></View></View>{card('Save consent','Apply your agreement on this device',
            /** Saves the policy agreement selected by the user. */
             ()=>{if(!consentAgreed){setNotice('Please tick the agreement checkbox first.');return;}if(model.consent.grant())setNotice('Your agreement has been saved on this device.');})}{card('Withdraw consent','Stop consent-based processing on this device',
            /** Opens the Withdraw consent panel. */
             ()=>{setNotice('');setPanel('Withdraw consent');})}{!!model.consent.error&&<Text accessibilityRole="alert" style={s.panelCopy}>{model.consent.error}</Text>}</>}{panel==='Withdraw consent'&&<><Text style={s.panelCopy}>Withdraw your agreement? Document analysis and personalisation will pause on this device. Your account and files will remain.</Text>{card('Confirm withdrawal','Withdraw consent now',
            /** Closes the selected dialog or detail view. */
             ()=>{if(model.withdrawConsent())setPanel('');})}{card('Keep consent','Return without changing your choice',
            /** Opens the Manage consent panel. */
             ()=>setPanel('Manage consent'))}{!!model.consent.error&&<Text accessibilityRole="alert" style={s.panelCopy}>{model.consent.error}</Text>}</>}<Pressable accessibilityRole="button" accessibilityLabel="Close settings dialog" style={s.close} onPress={
            /** Closes the selected dialog or detail view. */
             () => setPanel('')}><Text style={s.closeText}>Close</Text></Pressable></ScrollView></View></View></Modal>
    </View></View>;
}
const base = StyleSheet.create({
    safe:{flex:1,backgroundColor:'#000'},canvas:{flex:1,width:'100%',maxWidth:480,alignSelf:'center',overflow:'hidden'},content:{paddingHorizontal:20,flexGrow:1},header:{flexDirection:'row',alignItems:'center',minHeight:44,marginBottom:17,paddingRight:14},back:{minWidth:44,minHeight:44,alignItems:'flex-start',justifyContent:'center'},title:{color:'#fff',fontSize:32,fontWeight:'700',flex:1,marginLeft:24},profile:{minHeight:100,borderWidth:1,borderColor:'#353e44',borderRadius:10,backgroundColor:'rgba(6,15,20,.85)',flexDirection:'row',alignItems:'center',paddingHorizontal:17,gap:26,marginBottom:10},profileCopy:{flex:1,paddingVertical:10},cardTitle:{color:'#fff',fontSize:13,fontWeight:'700',lineHeight:16},student:{color:'#a6a6a4',fontSize:13,marginTop:10},group:{color:'#fff',fontSize:13,fontWeight:'700',marginBottom:8},cardIcon:{position:'absolute',left:4,top:3,width:43,height:43,opacity:.7},card:{minHeight:50,justifyContent:'center',paddingVertical:5,paddingLeft:64,paddingRight:10,borderWidth:1,borderColor:'#353e44',borderRadius:10,backgroundColor:'rgba(6,15,20,.85)',marginBottom:10},copy:{color:'#a6a6a4',fontSize:13,lineHeight:17,marginTop:6},logout:{minHeight:44,flexDirection:'row',alignItems:'center',justifyContent:'center',gap:8,marginTop:0},logoutText:{color:'rgba(225,19,19,0.7)',fontSize:13,fontWeight:'700'},scrim:{flex:1,backgroundColor:'rgba(0,0,0,.75)',justifyContent:'center',alignItems:'center',padding:20},resourceItem:{minHeight:50,padding:12,borderWidth:1,borderColor:'#353e44',borderRadius:10,backgroundColor:'#060f14',justifyContent:'center'},panel:{width:'100%',maxWidth:420,padding:20,borderRadius:16,borderWidth:1,borderColor:'#353e44',backgroundColor:'#0b151c'},panelTitle:{fontSize:20,fontWeight:'700',color:'#e8b866',marginBottom:20},panelCopy:{fontSize:14,lineHeight:22,color:'#a6a6a4',marginBottom:20},close:{minHeight:44,alignItems:'center',justifyContent:'center',backgroundColor:'#d4af53',borderRadius:8},closeText:{color:'#000',fontSize:14,fontWeight:'700'},
});
