import type {BundledAsset} from '../platform/assets';
/** Displays details and actions for the selected career opportunity. */
import ScreenHeader, {screenHeaderTop} from '../components/ScreenHeader';
import VignettePair from '../components/VignettePair';
import { useEffect, useState } from 'react';
import { Image, Linking, Pressable, ScrollView, StyleSheet, View } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import Text from '../components/Text';
import LocalVector from '../onboarding/LocalVector';
import OpportunityIcon from '../opportunities/OpportunityIcon';
import useResponsiveStyles from '../layout/useResponsiveStyles';
import type { AppModel } from '../navigation/useAppModel';

/** Displays details and actions for the selected career opportunity. */
export default function JobDetailScreen({ model }: { model: AppModel }) {
    const s = useResponsiveStyles(base);
    const scale = Math.min(model.width, 480) / 408.393;
    const insets = useSafeAreaInsets();
    const jobs = model.opportunityModel;
    const opportunity = jobs.selected;
    const [notice, setNotice] = useState('');
    useEffect(() => { setNotice(''); }, [opportunity?.id, opportunity?.applicationUrl]);
    const saved = !!opportunity && jobs.saved.includes(opportunity.id);
    /** Opens the selected opportunity application link, or displays an availability message. */
    const apply =  async () => {
        setNotice('');
        const url = opportunity?.applicationUrl;
        if (!url || !/^https?:\/\//i.test(url)) { setNotice('A company application link is not available yet.'); return; }
        try { await Linking.openURL(url); } catch { setNotice('Unable to open the application link. Please try again.'); }
    };
    /** Displays bundled vector artwork at the calculated screen scale. */
    const vector =  (source: BundledAsset, height: number) => <View style={{ width: 414.8, height, transformOrigin: 'top left', transform: [{ scale }] }}><LocalVector source={source} width={414.8} height={height}/></View>;
    return <View style={s.safe}><View style={s.canvas}>
        <View pointerEvents="none" style={StyleSheet.absoluteFill}>
            <Image testID="intel-city" source={{uri:'/assets/opportunity-intel/city.png'}} style={s.city} resizeMode="cover"/>
            <VignettePair height={885.372*scale}/>

            <View style={{ position: 'absolute', top: -14 * scale, opacity: .21 }}>{vector({uri:'/assets/opportunity-intel/upper.svg'},186.551)}</View>
            <View style={{ position: 'absolute', bottom: 0, opacity: .21 }}>{vector({uri:'/assets/opportunity-intel/lower.svg'},310.065)}</View>
        </View>
        <ScrollView contentContainerStyle={[s.content, { paddingTop: screenHeaderTop(scale,insets.top), paddingBottom: 115 * scale + insets.bottom }]}>
            <ScreenHeader title="OPPORTUNITY INTEL" onBack={
                /** Opens the jobs screen. */
                 ()=>model.goBack('jobs')} backLabel="Back to opportunities" scale={scale}/>
            <View style={s.heroSpace}/>
            {opportunity ? <>
                <View style={s.badges}><View style={s.match}><Text style={s.matchText}>{opportunity.match === null ? 'Match unavailable' : `${opportunity.match}% Match`}</Text></View><View style={s.type}><Text style={s.typeText}>{opportunity.employmentType}</Text></View></View>
                <Text style={s.section}>Recommended matches</Text>
                <View style={s.card}><View style={s.row}><Text style={s.company}>{opportunity.company}</Text><Pressable accessibilityRole="button" accessibilityLabel={saved ? 'Unsave opportunity' : 'Save opportunity'} accessibilityState={{ selected: saved }} onPress={
                    /** Adds or removes the selected item from saved entries. */
                     () => jobs.toggleSaved(opportunity.id)} style={s.iconButton}><OpportunityIcon name="bookmark" size={18 * scale} filled={saved}/></Pressable></View><Text style={s.job}>{opportunity.title}</Text><Text style={s.muted}>{opportunity.area}, {opportunity.region} | {opportunity.workModel}</Text></View>
                <Text style={s.section}>The mission brief</Text><Text style={s.description}>{opportunity.description || 'Job description has not been provided.'}</Text>
                <View style={s.skillsHeading}><Text style={s.section}>Required Skills</Text><Text style={s.gold}>YOUR MATCH</Text></View>
                <View style={s.skills}>{opportunity.requiredSkills?.length ? opportunity.requiredSkills.map(
                    /** Renders a skill entry from its data. */
                     (skill,index) => <View key={`${skill.name}-${index}`} style={[s.skill, { backgroundColor: skill.status === 'matched' ? 'rgba(33,96,80,.7)' : skill.status === 'partial' ? '#3c2e1c' : '#3e2724' }]}><Text style={{ fontSize: 12 * scale, color: skill.status === 'matched' ? '#6de0b3' : skill.status === 'partial' ? '#c6a572' : '#f6a28d' }}>{skill.name}{skill.status === 'matched' ? '  ✓' : ''}</Text></View>) : <Text style={s.muted}>Required skills have not been provided.</Text>}</View>
                <Text style={s.section}>Why you’re a match</Text>
                <View style={s.intel}><Image source={{uri:'/assets/opportunity-intel/intel.png'}} style={s.intelImage}/><View style={s.intelCopy}><Text style={s.company}>AI Career Intel</Text><Text style={s.intelText}>{opportunity.careerIntel || 'Career insights will appear when analysis is available.'}</Text></View></View>
                <Pressable accessibilityRole="link" onPress={apply} style={s.apply}><Text style={s.applyText}>Apply on Company Site</Text><OpportunityIcon name="arrow" size={24 * scale} color="#000"/></Pressable>
            </> : <View style={s.card}><Text style={s.company}>No opportunity selected</Text><Text style={s.description}>Select an available opportunity to view its details.</Text><Pressable onPress={
                /** Opens the jobs screen. */
                 () => model.go('jobs')} accessibilityRole="button" style={s.iconButton}><Text style={s.gold}>View opportunities</Text></Pressable></View>}
            {!!jobs.error && <Text accessibilityRole="alert" style={s.notice}>{jobs.error}</Text>}
            {!!notice && <Text accessibilityRole="alert" style={s.notice}>{notice}</Text>}
        </ScrollView>
    </View></View>;
}
const base = StyleSheet.create({
    safe:{flex:1,backgroundColor:'#000'},canvas:{flex:1,width:'100%',maxWidth:480,alignSelf:'center',overflow:'hidden'},city:{position:'absolute',top:9,left:-24,width:507,height:285},content:{paddingHorizontal:20,flexGrow:1},header:{flexDirection:'row',alignItems:'center',marginLeft:-8},iconButton:{minWidth:44,minHeight:44,justifyContent:'center',alignItems:'center'},title:{flex:1,fontSize:32,fontWeight:'700',color:'#fff'},heroSpace:{height:143},badges:{flexDirection:'row',gap:14,marginBottom:16},match:{backgroundColor:'rgba(33,96,80,.7)',paddingHorizontal:12,paddingVertical:3,borderRadius:5},matchText:{color:'#6de0b3',fontSize:12},type:{backgroundColor:'#3c2e1c',paddingHorizontal:12,paddingVertical:3,borderRadius:5},typeText:{color:'#c6a572',fontSize:12},section:{color:'#fff',fontSize:13,fontWeight:'700',marginBottom:16},card:{backgroundColor:'rgba(6,15,20,.85)',borderColor:'#353e44',borderWidth:1,borderRadius:10,paddingHorizontal:13,paddingBottom:10,marginBottom:18},row:{flexDirection:'row',alignItems:'center',justifyContent:'space-between'},company:{color:'#fff',fontSize:13,fontWeight:'700',flexShrink:1},job:{color:'#fff',fontSize:13,fontWeight:'700',marginTop:2},muted:{color:'#a6a6a4',fontSize:13,lineHeight:18},description:{color:'#a6a6a4',fontSize:13,lineHeight:19,marginBottom:18},skillsHeading:{flexDirection:'row',alignItems:'flex-start',justifyContent:'space-between'},gold:{color:'#d4af53',fontSize:13,fontWeight:'700'},skills:{flexDirection:'row',flexWrap:'wrap',gap:12,marginBottom:22},skill:{paddingHorizontal:12,paddingVertical:3,borderRadius:5,minWidth:72},intel:{backgroundColor:'rgba(6,15,20,.85)',borderColor:'#353e44',borderWidth:1,borderRadius:10,padding:13,minHeight:91,flexDirection:'row',alignItems:'center',gap:14,marginBottom:20},intelImage:{width:63,height:63},intelCopy:{flex:1},intelText:{fontSize:13,lineHeight:19,color:'#a6a6a4',marginTop:8},apply:{backgroundColor:'#c4872d',borderColor:'#ba7e27',borderWidth:1,borderRadius:10,minHeight:54,marginHorizontal:4,flexDirection:'row',gap:12,alignItems:'center',justifyContent:'center',padding:10},applyText:{color:'#000',fontFamily:'PoppinsMedium',fontSize:16},notice:{fontSize:13,lineHeight:19,color:'#e8b866',marginTop:12},nav:{position:'absolute',bottom:0,left:0,right:0,paddingTop:7,paddingHorizontal:12,backgroundColor:'rgba(0,0,0,.94)',borderTopWidth:1,borderColor:'#a58a39',flexDirection:'row',justifyContent:'space-around'},navItem:{flex:1,minHeight:64,alignItems:'center',justifyContent:'center'},navIcon:{minWidth:44,minHeight:44,borderRadius:10,alignItems:'center',justifyContent:'center'},navText:{color:'#a6a6a4',fontSize:12,marginTop:4},
});
