import type {BundledAsset} from '../platform/assets';
/** Displays role matching scores and the individual skill breakdown. */
import VignettePair from '../components/VignettePair';
import ScreenHeader, {screenHeaderTop} from '../components/ScreenHeader';
import { useEffect, useState } from 'react';
import { Platform, BackHandler, Image, Modal, Pressable, ScrollView, StyleSheet, View } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import Svg, { Circle } from 'react-native-svg';
import Text from '../components/Text';
import ChevronRight from '../components/ChevronRight';
import OpportunityIcon from '../opportunities/OpportunityIcon';
import LocalVector from '../onboarding/LocalVector';
import useResponsiveStyles from '../layout/useResponsiveStyles';
import type { AppModel } from '../navigation/useAppModel';

/** Displays role matching scores and the individual skill breakdown. */
export default function SkillsMatchScreen({model}:{model:AppModel}) {
    const roles=model.roles;
    const s=useResponsiveStyles(base),insets=useSafeAreaInsets(),scale=Math.min(model.width,480)/408.393;
    const [selectRole,setSelectRole]=useState(false);
    const match=model.careerAssessment?.skillMatch;
    const result=match&&match.role===model.role&&Number.isFinite(match.overall)&&match.overall>=0&&match.overall<=100?match:null;
    /** Checks that a skill has a valid score between zero and one hundred. */
    const skills=(result?.skills??model.careerAssessment?.skills??[]).filter( skill=>Number.isFinite(skill.score)&&skill.score>=0&&skill.score<=100);
    const score=result?.overall??null;
    /** Displays bundled vector artwork at the calculated screen scale. */
    const vector= (source:BundledAsset,height:number)=><View style={{width:408.8,height,transformOrigin:'top left',transform:[{scale}]}}><LocalVector source={source} width={408.8} height={height}/></View>;
    useEffect(
        /** Registers the hardware back action and its cleanup. */
         ()=>{
    /** Handles the device back action for this screen. */
    if (Platform.OS === 'web') return;
    const back=BackHandler.addEventListener('hardwareBackPress', ()=>{model.goBack('analysis');return true;});
    /** Removes the registered listeners when the effect ends. */
    return  ()=>back.remove();},[model.goBack]);
    return <View style={s.safe}><View testID="skills-match-screen" style={s.canvas}>
        <View pointerEvents="none" style={StyleSheet.absoluteFill}>
            <Image testID="skills-match-landscape" source={{uri:'/assets/roadmap/landscape.png'}} resizeMode="stretch" style={{position:'absolute',left:0,top:.6*scale,width:412.35*scale,height:232.06*scale}}/>
            <VignettePair height={885.372*scale}/>
            <View style={{position:'absolute',top:.22*scale,left:.22*scale,opacity:.35}}>{vector({uri:'/assets/home/upper.svg'},186.551)}</View>
            <View style={{position:'absolute',bottom:0,left:.22*scale,opacity:.35}}>{vector({uri:'/assets/home/lower.svg'},310.065)}</View>
        </View>
        <ScrollView testID="skills-match-content" contentContainerStyle={[s.content,{paddingTop:screenHeaderTop(scale,insets.top),paddingBottom:Math.max(25*scale,insets.bottom+8)}]}>
            <ScreenHeader title="SKILLS MATCH" onBack={
                /** Opens the analysis screen. */
                 ()=>model.goBack('analysis')} backLabel="Back to CV analysis" scale={scale}/>
            <Text style={s.eyebrow}>YOUR CURRENT LOADOUT</Text>
            <Pressable accessibilityRole="button" accessibilityLabel="Choose target role" onPress={
                /** Handles the on press for this component. */
                 ()=>setSelectRole(true)} style={[s.card,s.roleCard]}><View style={s.roleIcon}><Image source={{uri:'/assets/cv-analysis/b1e5a.png'}} resizeMode="stretch" style={{position:'absolute',width:61.77*scale,height:48.97*scale,left:-12.07*scale,top:-7.72*scale}}/></View><View style={s.copy}><Text style={s.label}>Target Role</Text><Text style={s.small}>{roles[model.role] || model.careerAssessment?.targetCareer || 'Set your target roles'}</Text></View><ChevronRight size={20*scale} color="#d4af53"/></Pressable>
            <View style={s.tabs}><Pressable accessibilityRole="tab" accessibilityState={{selected:false}} accessibilityLabel="CV Analysis" onPress={
                /** Opens the analysis screen. */
                 ()=>model.go('analysis')} style={s.tab}><Text style={s.label}>CV Analysis</Text></Pressable><View accessibilityRole="tab" accessibilityState={{selected:true}} style={[s.tab,s.selectedTab]}><Text style={s.selectedTabText}>Skills Match</Text></View></View>
            <View style={[s.card,s.scoreCard]}><View testID="skill-match-ring" accessibilityLabel={score===null?'Overall skill match pending':`Overall skill match ${score}%`} style={s.ring}><Svg width={94*scale} height={94*scale} viewBox="0 0 94 94"><Circle cx={47} cy={47} r={44} fill="none" stroke="#d1d1d6" strokeWidth={6}/>{score!==null&&<Circle cx={47} cy={47} r={44} fill="none" stroke="#d4af53" strokeWidth={6} strokeDasharray={`${2*Math.PI*44*score/100} ${2*Math.PI*44}`} transform="rotate(-90 47 47)"/>}</Svg><View style={s.ringText}><Text testID="overall-skill-match" style={s.score}>{score===null?'—':score+'%'}</Text><Text style={s.ringCaption}>Skill Match</Text></View></View><View style={s.copy}><Text style={s.goldHeading}>OVERALL SKILL MATCH</Text><Text style={s.scoreDescription}>{result?.summary||'Role matching is pending. Existing skill proficiency is shown below.'}</Text></View></View>
            <View style={s.breakdownHeader}><Text style={s.heading}>Skill Breakdown</Text><Text testID="skills-count" style={s.goldHeading}>{skills.length} SKILLS</Text></View>
            <View style={s.breakdown}>{!skills.length&&<View style={s.emptyState}>
                <Text style={s.empty}>No skill data is available yet. Analyse your CV to add detected skills, or select a target role to see its requirements.</Text>
                <Pressable accessibilityRole="button" onPress={()=>model.go('analysis')} style={s.emptyAction}><Text style={s.emptyActionText}>Go to CV Analysis</Text></Pressable>
            </View>}{skills.map(
                /** Renders a skill entry from its data. */
                 (skill,index)=>{
                const strong=skill.score>=80,building=skill.score>=60;
                const color=strong?'#6de0b3':building?'#d4af53':'#f6a28d';
                const status='status' in skill?skill.status:'level' in skill?skill.level:'UP NEXT';
                return <View key={skill.name+'-'+index}><View testID={'skill-row-'+index} style={s.skillRow}><View style={s.skillTop}><Text style={s.skillName}>{skill.name}</Text><View style={[s.skillBadge,{backgroundColor:strong?'#163a31':building?'#3c2e1c':'#3e2724'}]}><Text style={[s.skillBadgeText,{color:strong?'#55c6a0':building?'#d4af53':'#e17864'}]}>{status.toUpperCase()}</Text></View></View><View style={s.progressRow}><View accessibilityRole="progressbar" accessibilityLabel={skill.name+(result?' match':' proficiency')} accessibilityValue={{min:0,max:100,now:skill.score}} style={s.track}><View style={[s.fill,{width:`${skill.score}%`,backgroundColor:color}]}/></View><Text style={s.matchText}>{skill.score}% {result?'match':'proficiency'}</Text></View></View>{index<skills.length-1&&<View style={{height:scale}}><View style={{width:329,height:1,transformOrigin:'top left',transform:[{scaleX:333*scale/329},{scaleY:scale}]}}><LocalVector source={{uri:'/assets/skills-match/divider.svg'}} width={329} height={1}/></View></View>}</View>;
            })}</View>
            <View style={{flex:1,minHeight:17*scale}}/>
            <Pressable accessibilityRole="button" accessibilityLabel="Find Learning Paths" onPress={
                /** Opens the learning screen. */
                 ()=>model.go('learning')} style={s.continue}><Text style={s.continueText}>Find Learning Paths</Text><OpportunityIcon name="arrow" size={24*scale} color="#000"/></Pressable>
        </ScrollView>
        <Modal visible={selectRole} transparent animationType="fade" onRequestClose={
            /** Handles the on request close for this component. */
             ()=>setSelectRole(false)}><View style={s.backdrop}><View accessibilityViewIsModal style={s.dialog}><ScrollView contentContainerStyle={{padding:20}}><Text accessibilityRole="header" style={s.dialogTitle}>Target Role</Text>{!roles.length&&<Pressable accessibilityRole="button" onPress={
            /** Opens the profile settings screen. */
             ()=>{setSelectRole(false);model.go('profileSettings');}} style={s.roleOption}><Text style={s.goldHeading}>Add target roles in Profile Settings</Text></Pressable>}{roles.map(
            /** Renders a role entry from its data. */
             (role,index)=><Pressable key={role} accessibilityRole="radio" accessibilityState={{checked:index===model.role}} onPress={
            /** Handles the on press for this component. */
             ()=>{model.setRole(index);setSelectRole(false);}} style={s.roleOption}><Text style={s.label}>{role}</Text><Text style={s.goldHeading}>{model.role===index?'✓':''}</Text></Pressable>)}<Pressable accessibilityRole="button" accessibilityLabel="Close role selection" onPress={
            /** Handles the on press for this component. */
             ()=>setSelectRole(false)} style={[s.continue,{marginTop:20}]}><Text style={s.continueText}>Close</Text></Pressable></ScrollView></View></View></Modal>
    </View></View>;
}
const base=StyleSheet.create({
    safe:{flex:1,backgroundColor:'#000'},canvas:{flex:1,width:'100%',maxWidth:480,alignSelf:'center',overflow:'hidden'},content:{flexGrow:1,paddingHorizontal:23},header:{flexDirection:'row',alignItems:'center',marginLeft:-9,minHeight:44},iconButton:{minWidth:44,minHeight:44,alignItems:'center',justifyContent:'center'},title:{fontSize:30,fontWeight:'600',color:'#fff',letterSpacing:-.8},card:{borderWidth:1,borderColor:'#353e44',borderRadius:10,backgroundColor:'rgba(6,15,20,.78)'},roleCard:{marginTop:55,minHeight:66,padding:11,flexDirection:'row',alignItems:'center',gap:10},roleIcon:{width:38,height:38,overflow:'hidden'},copy:{flex:1},label:{fontSize:12,lineHeight:16,color:'#fff'},small:{fontSize:10,lineHeight:16,color:'#d1d1d6',marginTop:4},heading:{fontSize:12,lineHeight:16,fontWeight:'700',color:'#fff'},goldHeading:{fontSize:12,lineHeight:16,fontWeight:'700',color:'#d4af53'},tabs:{flexDirection:'row',gap:16,marginTop:16,marginBottom:11},tab:{minHeight:46,minWidth:96,paddingHorizontal:10,justifyContent:'center',alignItems:'center',backgroundColor:'#090e11',borderWidth:1,borderColor:'#d4af53',borderRadius:10},selectedTab:{backgroundColor:'#d4af53'},selectedTabText:{fontSize:13,lineHeight:16,fontWeight:'700',color:'#000'},scoreCard:{minHeight:135,flexDirection:'row',alignItems:'center',gap:32,paddingHorizontal:22,paddingVertical:18},ring:{width:94,height:94,alignItems:'center',justifyContent:'center'},ringText:{position:'absolute',alignItems:'center'},score:{fontSize:20,lineHeight:24,color:'#fff',fontWeight:'700'},ringCaption:{fontSize:10,lineHeight:16,color:'#d1d1d6'},scoreDescription:{fontSize:10,lineHeight:22,color:'#d1d1d6',marginTop:5},sectionRow:{flexDirection:'row',alignItems:'center',justifyContent:'space-between',minHeight:37},link:{minHeight:44,flexDirection:'row',alignItems:'center',gap:5},fileCard:{minHeight:66,flexDirection:'row',alignItems:'center',gap:10,padding:12},fileIcon:{width:40,height:39},status:{paddingHorizontal:5,paddingVertical:2,borderRadius:5,backgroundColor:'rgba(33,96,80,.7)',maxWidth:90},statusText:{fontSize:12,lineHeight:16,color:'#6de0b3'},reportCard:{minHeight:74,flexDirection:'row',alignItems:'center',gap:10,paddingHorizontal:9,borderRadius:10,borderWidth:1,borderColor:'#765728',backgroundColor:'#252112'},aiIcon:{width:63,height:63},continue:{minHeight:54,borderRadius:10,borderWidth:1,borderColor:'#ba7e27',backgroundColor:'#c4872d',flexDirection:'row',alignItems:'center',justifyContent:'center',gap:20,padding:10},continueText:{fontFamily:'PoppinsMedium',fontSize:16,lineHeight:24,color:'#000'},backdrop:{flex:1,backgroundColor:'rgba(0,0,0,.8)',justifyContent:'center',padding:24},dialog:{width:'100%',maxWidth:440,maxHeight:'80%',alignSelf:'center',backgroundColor:'#060f14',borderColor:'#353e44',borderWidth:1,borderRadius:12},dialogTitle:{fontSize:20,fontWeight:'700',color:'#fff'},meta:{fontSize:13,lineHeight:20,color:'#d1d1d6',marginTop:16},roleOption:{minHeight:48,flexDirection:'row',justifyContent:'space-between',alignItems:'center',borderBottomWidth:1,borderBottomColor:'#353e44',paddingVertical:12},
    eyebrow:{fontSize:13,lineHeight:16,fontWeight:'700',color:'#d4af53',marginTop:22,marginLeft:6},breakdownHeader:{flexDirection:'row',justifyContent:'space-between',alignItems:'center',paddingHorizontal:12,marginTop:8,marginBottom:14},breakdown:{borderWidth:1,borderColor:'#353e44',borderRadius:10,backgroundColor:'rgba(6,15,20,.85)',paddingHorizontal:14},skillRow:{paddingVertical:12,minHeight:68},skillTop:{flexDirection:'row',alignItems:'center',justifyContent:'space-between',gap:8},skillName:{flex:1,fontSize:13,lineHeight:16,fontWeight:'700',color:'#fff'},skillBadge:{paddingHorizontal:7,paddingVertical:2,borderRadius:5,maxWidth:120},skillBadgeText:{fontSize:12,lineHeight:16},progressRow:{flexDirection:'row',alignItems:'center',gap:10,marginTop:7},track:{flex:1,height:6,borderRadius:50,backgroundColor:'#a6a6a4',overflow:'hidden'},fill:{height:'100%',borderRadius:50},matchText:{fontSize:12,lineHeight:16,color:'#a6a6a4'},emptyState:{paddingBottom:12},empty:{fontSize:13,lineHeight:20,color:'#a6a6a4',paddingVertical:20},emptyAction:{minHeight:40,alignItems:'center',justifyContent:'center',paddingHorizontal:12,marginBottom:8,alignSelf:'flex-start',borderRadius:8,backgroundColor:'#c4872d'},emptyActionText:{fontSize:13,fontWeight:'700',color:'#000'},
});
