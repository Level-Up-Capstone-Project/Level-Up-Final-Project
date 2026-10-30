import type {BundledAsset} from '../platform/assets';
/** Displays opportunity search, filters and matching results. */
import ScreenHeader, {screenHeaderTop} from '../components/ScreenHeader';
import NotificationBell from '../notifications/NotificationBell';
import OpportunityNotifications from '../notifications/OpportunityNotifications';
import { useEffect, useState } from 'react';
import { Image, Modal, Pressable, ScrollView, StyleSheet, TextInput, View } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import VignettePair from '../components/VignettePair';
import Text from '../components/Text';
import LocalVector from '../onboarding/LocalVector';
import OpportunityIcon from '../opportunities/OpportunityIcon';
import ViewMore from '../opportunities/ViewMore';
import useResponsiveStyles from '../layout/useResponsiveStyles';
import type { AppModel } from '../navigation/useAppModel';
import type { EmploymentType, WorkModel } from '../opportunities/data';

/** Displays opportunity search, filters and matching results. */
export default function JobsScreen({ model }: { model: AppModel }) {
    const s = useResponsiveStyles(base);
    const insets = useSafeAreaInsets();
    const scale = Math.min(model.width, 480) / 408.393;
    const jobs = model.opportunityModel;
    const [filters, setFilters] = useState(false);
    const [notice, setNotice] = useState('');
    const [notificationsOpen,setNotificationsOpen] = useState(false);
    const notifications = model.opportunityNotifications;
    const [visibleCount, setVisibleCount] = useState(3);
    useEffect(
        /** Synchronises component state with the effect dependencies. */
         () => { setVisibleCount(3); }, [jobs.query, jobs.workModel, jobs.employmentType, jobs.locationOnly, jobs.savedOnly, jobs.location]);
    const hasFilters = jobs.workModel !== 'All' || jobs.employmentType !== 'All' || jobs.locationOnly || jobs.savedOnly;
    /** Displays bundled vector artwork at the calculated screen scale. */
    const vector =  (source: BundledAsset, width: number, height: number) => <View style={{ width, height, transformOrigin: 'top left', transform: [{ scale }] }}><LocalVector source={source} width={width} height={height}/></View>;
    /** Displays a selectable opportunity-filter option. */
    const chip =  (value: string, selected: boolean, onPress: () => void) => <Pressable key={value} accessibilityRole="button" accessibilityState={{ selected }} onPress={onPress} style={[s.option, selected && s.optionSelected]}><Text style={[s.optionText, selected && { color: '#000' }]}>{value}</Text></Pressable>;
    return <View style={s.safe}><View style={s.canvas}>
        <View pointerEvents="none" style={StyleSheet.absoluteFill}>
            <Image testID="opportunities-landscape" source={{uri:'/assets/opportunities/landscape.png'}} resizeMode="cover" style={{ position: 'absolute', left: -31 * scale, top: -4 * scale, width: 467 * scale, height: 263 * scale }}/>
            <VignettePair height={885.372*scale}/>

            <View testID="opportunities-upper-contours" style={{ position: 'absolute', top: -14 * scale, opacity: .21 }}>{vector({uri:'/assets/opportunities/upper-contours.svg'}, 408.8, 186.551)}</View>
            <View testID="opportunities-lower-contours" style={{ position: 'absolute', bottom: 0, opacity: .21 }}>{vector({uri:'/assets/opportunities/lower-contours.svg'}, 408.8, 310.065)}</View>
        </View>
        <ScrollView keyboardShouldPersistTaps="handled" contentContainerStyle={[s.content, { paddingTop: screenHeaderTop(scale,insets.top), paddingBottom: 110 * scale + insets.bottom }]}>
            <ScreenHeader title="OPPORTUNITIES" onBack={
                /** Opens the home screen. */
                 ()=>model.goBack('home')} backLabel="Back to previous page" scale={scale}/>
            {jobs.loading && <Text accessibilityLiveRegion="polite" style={s.notice}>Loading opportunities...</Text>}
            {!!jobs.error && <Text accessibilityRole="alert" style={s.notice}>{jobs.error}</Text>}
            {!!notice && <Text accessibilityRole="alert" style={s.notice}>{notice}</Text>}
            <View style={s.search}><OpportunityIcon name="search" size={23 * scale} color="#b9b9b9"/><TextInput accessibilityLabel="Search opportunities" placeholder="Search jobs, companies, internships..." placeholderTextColor="#777b7e" value={jobs.query} onChangeText={jobs.setQuery} style={s.input}/><Pressable accessibilityRole="button" accessibilityLabel="Filter opportunities" accessibilityState={{ expanded: filters }} onPress={
                /** Handles the on press for this component. */
                 () => setFilters(true)} style={s.iconButton}><OpportunityIcon name="filter" size={23 * scale}/></Pressable></View>
            <View style={[s.tabs,{alignItems:'center'}]}>{chip('For You', !jobs.savedOnly && !jobs.locationOnly,
                /** Displays feedback for the selected action. */
                 () => { jobs.setSavedOnly(false); jobs.setLocationOnly(false); setNotice(''); })}{chip('Location', jobs.locationOnly,
                /** Displays feedback for the selected action. */
                 () => { jobs.setLocationOnly(true); jobs.setSavedOnly(false); setNotice(jobs.location ? `Opportunities in ${jobs.location}` : 'No sign-up location is saved for this account.'); })}{hasFilters && <Pressable accessibilityRole="button" accessibilityLabel="Clear opportunity filters" onPress={
                /** Displays feedback for the selected action. */
                 () => { jobs.reset(); setNotice(''); }} style={s.clear}><Text style={s.gold}>Reset</Text></Pressable>}<View style={{flex:1}}/><NotificationBell unread={notifications.unread} onPress={
                /** Updates whether the notification inbox is visible. */
                 ()=>setNotificationsOpen(true)}/></View>
            <View style={s.resultsHeader}><Text style={s.sectionTitle}>{jobs.savedOnly ? 'Saved opportunities' : jobs.locationOnly ? 'Opportunities near you' : 'Recommended matches'}</Text><Text accessibilityLiveRegion="polite" testID="opportunity-count" style={s.count}>{jobs.results.length} RESULTS</Text></View>
            {jobs.results.slice(0, visibleCount).map(
                /** Renders a opportunity entry from its data. */
                 item => <View key={item.id} testID={`opportunity-${item.id}`} style={s.card}>
                <View style={s.companyRow}><Text style={s.company}>{item.company}</Text><Pressable accessibilityRole="button" accessibilityLabel={`${jobs.saved.includes(item.id) ? 'Unsave' : 'Save'} ${item.title}`} accessibilityState={{ selected: jobs.saved.includes(item.id) }} onPress={
                    /** Adds or removes the selected item from saved entries. */
                     () => jobs.toggleSaved(item.id)} style={s.save}><OpportunityIcon name="bookmark" size={18 * scale} filled={jobs.saved.includes(item.id)}/></Pressable></View>
                <Text style={s.jobTitle}>{item.title}</Text>
                <Text style={s.metadata}>{item.area}, {item.region} | {item.workModel}</Text>
                <Text style={s.employment}>{item.employmentType}</Text>
                <View style={s.cardBottom}><View style={s.match}><Text style={s.matchText}>{item.match === null ? 'Match unavailable' : `${item.match}% Match`}</Text></View><Pressable accessibilityRole="button" accessibilityLabel={`View ${item.title}`} onPress={
                    /** Opens the job screen. */
                     () => { jobs.setSelected(item); model.setJob(item.title); model.go('job'); }} style={s.view}><Text style={s.viewText}>View Opportunity</Text><OpportunityIcon name="arrow" size={16 * scale} color="#8d9294"/></Pressable></View>
            </View>)}
            {visibleCount < jobs.results.length && <ViewMore onPress={
                /** Handles the on press for this component. */
                 () => setVisibleCount(
                /** Builds the next visible count value from its previous state. */
                 count => count + 3)}/>}
            {!jobs.results.length && <View style={s.empty}><Text style={s.company}>No opportunities found</Text><Text style={s.metadata}>{jobs.locationOnly && !jobs.location ? 'Add your area or region during sign-up to use Location.' : 'No opportunities are available yet. Results will appear when a source is connected.'}</Text><Pressable accessibilityRole="button" onPress={
                /** Displays feedback for the selected action. */
                 () => { jobs.reset(); setNotice(''); }} style={s.option}><Text style={s.gold}>Clear filters</Text></Pressable></View>}

        </ScrollView>
        <OpportunityNotifications visible={notificationsOpen} onClose={
            /** Updates whether the notification inbox is visible. */
             ()=>setNotificationsOpen(false)} alerts={notifications.alerts} unread={notifications.unread} onMarkAll={notifications.markAllRead} availableIds={jobs.allOpportunities.map(
            /** Transforms each opportunitie into the required display or state value. */
             item=>item.id)} onOpen={
            /** Opens the job screen. */
             alert=>{
            notifications.markRead(alert.id);
            /** Finds the opportunity referenced by the selected notification. */
            const item=jobs.allOpportunities.find( opportunity=>opportunity.id===alert.opportunityId);
            setNotificationsOpen(false);
            if(item){jobs.setSelected(item);model.setJob(item.title);model.go('job');}
            else setNotice('This opportunity is currently unavailable.');
        }}/>
        <Modal visible={filters} transparent animationType="slide" onRequestClose={
            /** Handles the on request close for this component. */
             () => setFilters(false)}><View style={s.scrim}><View style={[s.sheet, { paddingBottom: Math.max(20, insets.bottom) }]}><ScrollView keyboardShouldPersistTaps="handled"><View style={s.resultsHeader}><Text accessibilityRole="header" style={s.sheetTitle}>Filter opportunities</Text><Pressable accessibilityRole="button" accessibilityLabel="Close filters" onPress={
            /** Handles the on press for this component. */
             () => setFilters(false)} style={s.iconButton}><Text style={s.gold}>✕</Text></Pressable></View>
            <Text style={s.filterLabel}>Working model</Text><View style={s.options}>{(['All','Remote','Hybrid','On-site'] as const).map(
                /** Transforms each opportunity into the required display or state value. */
                 value => chip(value, jobs.workModel === value,
                /** Sets the working-model filter. */
                 () => jobs.setWorkModel(value as WorkModel | 'All')))}</View>
            <Text style={s.filterLabel}>Employment type</Text><View style={s.options}>{(['All','Contract','Full-time','Part-time'] as const).map(
                /** Transforms each opportunity into the required display or state value. */
                 value => chip(value, jobs.employmentType === value,
                /** Sets the employment-type filter. */
                 () => jobs.setEmploymentType(value as EmploymentType | 'All')))}</View>

            <View style={s.options}>{chip('Saved only', jobs.savedOnly,
                /** Toggles the saved-opportunities filter. */
                 () => jobs.setSavedOnly(!jobs.savedOnly))}<Pressable accessibilityRole="button" onPress={
                /** Displays feedback for the selected action. */
                 () => { jobs.reset(); setNotice(''); }} style={s.option}><Text style={s.gold}>Reset filters</Text></Pressable></View>
            <Pressable accessibilityRole="button" accessibilityLabel="Show filtered opportunities" onPress={
                /** Handles the on press for this component. */
                 () => setFilters(false)} style={s.done}><Text style={s.doneText}>Show {jobs.results.length} results</Text></Pressable>
        </ScrollView></View></View></Modal>
    </View></View>;
}
const base = StyleSheet.create({
    safe:{flex:1,backgroundColor:'#000'},canvas:{flex:1,width:'100%',alignSelf:'stretch',overflow:'hidden'},content:{paddingHorizontal:20,flexGrow:1},
    header:{flexDirection:'row',alignItems:'center',gap:4,minHeight:44},iconButton:{minWidth:44,minHeight:44,alignItems:'center',justifyContent:'center'},title:{flex:1,color:'#b8b8b8',fontSize:32,fontWeight:'700'},notice:{color:'#d4af53',fontSize:12,marginTop:10},
    search:{marginTop:32,minHeight:66,paddingLeft:18,paddingRight:2,borderRadius:10,borderWidth:1,borderColor:'#353e44',backgroundColor:'rgba(6,15,20,.85)',flexDirection:'row',alignItems:'center',gap:15},input:{flex:1,minWidth:0,fontFamily:'Inter',fontSize:13,color:'#fff',paddingVertical:16},
    tabs:{flexDirection:'row',flexWrap:'wrap',alignItems:'center',gap:16,marginTop:16,marginBottom:16},option:{minHeight:46,minWidth:96,paddingHorizontal:18,paddingVertical:10,borderWidth:1,borderColor:'#9d813b',borderRadius:10,backgroundColor:'#090e11',alignItems:'center',justifyContent:'center'},optionSelected:{backgroundColor:'#a38b3e',borderColor:'#a38b3e'},optionText:{color:'#aaa',fontSize:13},clear:{minHeight:44,justifyContent:'center'},gold:{color:'#d4af53',fontSize:13},
    resultsHeader:{flexDirection:'row',alignItems:'center',justifyContent:'space-between',gap:12,marginBottom:16},sectionTitle:{color:'#b8b8b8',fontSize:13,fontWeight:'700',flexShrink:1},count:{color:'#a58a39',fontSize:13,fontWeight:'700'},
    card:{minHeight:148,paddingHorizontal:13,paddingTop:3,paddingBottom:4,borderRadius:10,borderWidth:1,borderColor:'#263238',backgroundColor:'rgba(6,15,20,.85)',marginBottom:16},companyRow:{flexDirection:'row',alignItems:'center',justifyContent:'space-between'},company:{color:'#b8b8b8',fontSize:13,fontWeight:'700',flexShrink:1},save:{minWidth:44,minHeight:40,alignItems:'flex-end',justifyContent:'center'},jobTitle:{color:'#b8b8b8',fontSize:13,fontWeight:'700',marginTop:2},metadata:{color:'#868b8e',fontSize:13,lineHeight:18},employment:{color:'#b19a64',fontSize:12,lineHeight:18},cardBottom:{flexDirection:'row',alignItems:'center',justifyContent:'space-between',gap:10,marginTop:2},match:{paddingHorizontal:9,paddingVertical:3,borderRadius:5,backgroundColor:'rgba(33,96,80,.7)'},matchText:{fontSize:12,color:'#6de0b3'},view:{minHeight:44,flexDirection:'row',alignItems:'center',gap:8},viewText:{fontSize:13,color:'#868b8e'},demo:{fontSize:11,color:'#777',textAlign:'center',marginTop:8},empty:{padding:20,gap:12,borderColor:'#353e44',borderWidth:1,borderRadius:10},
    nav:{position:'absolute',bottom:0,left:0,right:0,paddingTop:7,paddingHorizontal:12,backgroundColor:'rgba(0,0,0,.94)',borderTopWidth:1,borderColor:'#a58a39',flexDirection:'row',justifyContent:'space-around'},navItem:{flex:1,minHeight:64,alignItems:'center',justifyContent:'center'},navIcon:{minWidth:44,minHeight:44,borderRadius:10,alignItems:'center',justifyContent:'center'},navText:{color:'#a6a6a4',fontSize:12,marginTop:4},
    scrim:{flex:1,backgroundColor:'rgba(0,0,0,.7)',justifyContent:'flex-end'},sheet:{maxHeight:'85%',width:'100%',maxWidth:480,alignSelf:'center',backgroundColor:'#10191f',padding:20,borderTopLeftRadius:22,borderTopRightRadius:22,borderWidth:1,borderColor:'#353e44'},sheetTitle:{fontSize:20,fontWeight:'700',color:'#e8b866'},filterLabel:{fontSize:14,color:'#fff',marginTop:10,marginBottom:12},options:{flexDirection:'row',flexWrap:'wrap',gap:10,marginBottom:14},done:{minHeight:52,borderRadius:12,backgroundColor:'#d4af53',alignItems:'center',justifyContent:'center',marginTop:8},doneText:{fontSize:15,color:'#000',fontWeight:'600'},
});
