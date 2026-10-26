import type {BundledAsset} from '../platform/assets';
/** Displays the career dashboard, shortcuts and next mission. */
import VignettePair from '../components/VignettePair';
import MissionLandscape from '../dashboard/MissionLandscape';
import { useState } from 'react';
import { Image, Modal, Pressable, ScrollView, StyleSheet, View, useWindowDimensions } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import Svg, { Circle } from 'react-native-svg';
import Text from '../components/Text';
import LocalVector from '../onboarding/LocalVector';
import OpportunityIcon from '../opportunities/OpportunityIcon';
import useResponsiveStyles from '../layout/useResponsiveStyles';
import type { AppModel } from '../navigation/useAppModel';
import type { Screen } from '../navigation/types';

/** Displays the career dashboard, shortcuts and next mission. */
export default function HomeScreen({ model }: { model: AppModel }) {
    const {learningGroups,resourcesForTopic}=model;
    const responsive = useResponsiveStyles(base);
    const scale = Math.min(model.width,480) / 408.393;
    const insets = useSafeAreaInsets();
    const { height } = useWindowDimensions();
    const compact = (height-insets.top-insets.bottom)/scale < 880;
    const [viewportHeight,setViewportHeight] = useState(0);
    const [contentHeight,setContentHeight] = useState(0);
    const s = {
        ...responsive,
        subtitle:{...responsive.subtitle,marginTop:(compact ? 6 : 8)*scale,marginBottom:(compact ? 12 : 16)*scale},
        readinessCard:{...responsive.readinessCard,...(compact ? {paddingTop:14*scale,paddingBottom:10*scale} : {})},
        readinessRow:{...responsive.readinessRow,marginTop:(compact ? 12 : 14)*scale},
        divider:{...responsive.divider,marginTop:(compact ? 12 : 15)*scale},
        trendRow:{...responsive.trendRow,marginTop:(compact ? 8 : 10)*scale},
    };
    const [expanded,setExpanded] = useState(false);
    const [notice,setNotice] = useState('');
    const data = model.careerAssessment;
    const readiness = typeof data?.readiness === 'number' && Number.isFinite(data.readiness) ? Math.max(0,Math.min(100,data.readiness)) : null;
    const cvScore = model.cvAssessment?.score ?? data?.cvQualityScore ?? null;
    const displayScore = readiness ?? (model.careerAssessmentLoaded && typeof cvScore === 'number' && Number.isFinite(cvScore) ? Math.max(0,Math.min(100,cvScore)) : null);
    const scoreLabel = readiness !== null ? 'Career ready' : displayScore !== null ? 'CV score' : 'Not assessed';
    const trackedSkills = data?.trackedSkillCount;
    /** Collects the topic names from the learning groups. */
    const topics = learningGroups.flatMap( group => group.topics);
    /** Checks whether a topic is still unfinished. */
    const remaining = topics.filter( topic => !model.completed.includes(topic));
    const next = remaining.includes(model.topic) && model.taskProgress[model.topic]?.some(Boolean) ? model.topic : remaining[0];
    const total = next ? resourcesForTopic(next).length : 0;
    const done = next ? resourcesForTopic(next).filter(
        /** Selects the resources that match the current conditions. */
         (_,i) => !!model.taskProgress[next]?.[i]).length : 0;
    const missionNumber = next ? topics.indexOf(next)+1 : topics.length;
    /** Opens the next available mission or the learning roadmap. */
    const openMission =  () => { if(next) {model.setTopic(next);model.go('topic');} else model.go('learning'); };
    const docDescription = model.cvAnalysis.status === 'processing' || model.transcriptAnalysis.status === 'processing' ? 'Analysis in progress' : model.documentAnalysisEntry === 'analysis' ? 'Review your CV & transcript results' : model.documentAnalysisEntry === 'uploadTranscript' ? 'Step 2: add your transcript' : 'Step 1: add your CV';
    const matchCount = model.opportunityModel.allOpportunities.length;
    /** Displays bundled vector artwork at the calculated screen scale. */
    const vector =  (source:BundledAsset,width:number,height:number) => <View style={{width,height,transformOrigin:'top left',transform:[{scale}]}}><LocalVector source={source} width={width} height={height}/></View>;
    const shortcuts: {title:string;copy:string;icon:BundledAsset;route:Screen}[] = [
        {title:'Document Analysis',copy:docDescription,icon:{uri:'/assets/home/documents.png'},route:model.documentAnalysisEntry},
        {title:'Skill Gaps',copy:data ? `${data.prioritySkills.length} priority skills` : 'Assessment pending',icon:{uri:'/assets/home/skill-gaps.png'},route:'skills'},
        {title:'Learning Path',copy:!topics.length ? 'Recommendations pending' : remaining.length ? 'Continue your route' : 'Roadmap complete',icon:{uri:'/assets/home/learning.png'},route:'learning'},
        {title:'Opportunities',copy:`${matchCount} matches found`,icon:{uri:'/assets/home/opportunities.png'},route:'jobs'},
    ];
    return <View testID="home-dashboard" style={s.safe}><View style={s.canvas}>
        <View pointerEvents="none" style={StyleSheet.absoluteFill}>
            <Image testID="home-landscape" source={{uri:'/assets/home/landscape.png'}} style={s.landscape} resizeMode="cover"/>
            <View pointerEvents="none" style={{position:'absolute',top:0,left:0,right:0,height:240*scale,backgroundColor:'rgba(0,8,12,.58)'}}/>
            <VignettePair height={885.372*scale}/>
            <View style={{position:'absolute',top:0,opacity:.2}}>{vector({uri:'/assets/home/upper.svg'},408.8,186.551)}</View><View style={{position:'absolute',bottom:0,opacity:.2}}>{vector({uri:'/assets/home/lower.svg'},408.8,310.065)}</View>
        </View>
        <ScrollView style={{flex:1}} testID="home-content" onLayout={
            /** Records the measured layout dimensions for this view. */
             event => setViewportHeight(event.nativeEvent.layout.height)} onContentSizeChange={
            /** Records the measured layout dimensions for this view. */
             (_,value) => setContentHeight(value)} scrollEnabled={contentHeight > viewportHeight + 1} showsVerticalScrollIndicator={false} alwaysBounceVertical={false} contentContainerStyle={[s.content,{paddingTop:Math.max((compact ? 48 : 67)*scale,insets.top+8),paddingBottom:8*scale}]} >
            <View style={s.greeting}><View style={s.welcomeCopy}><Text style={s.welcome}>WELCOME BACK,</Text><View style={s.nameRow}><Text style={[s.name,{flexShrink:1}]}>{(model.account.name.trim().split(/\s+/)[0] || 'Explorer').toUpperCase()}.</Text><Image testID="home-wave-icon" accessible={false} source={{uri:'/assets/home/wave.png'}} resizeMode="contain" style={s.wave}/></View></View></View>
            <View style={s.readinessCard}><Text style={s.eyebrow}>CAREER READINESS</Text><View style={s.readinessRow}><View style={s.readinessCopy}><Text style={s.future}>{displayScore === null ? 'Your journey\nstarts here.' : 'Your future is\ntaking shape.'}</Text><Text style={s.stats}>{trackedSkills === undefined ? '—' : trackedSkills} skills tracked{'\n'}{remaining.length} missions active</Text></View><View style={s.ring}><Svg width="100%" height="100%" viewBox="0 0 94 94"><Circle cx={47} cy={47} r={44} fill="none" stroke="#c8c8c8" strokeWidth={6}/>{displayScore !== null && <Circle testID="readiness-arc" cx={47} cy={47} r={44} fill="none" stroke="#d4af53" strokeWidth={6} strokeDasharray={`${2*Math.PI*44}`} strokeDashoffset={2*Math.PI*44*(1-displayScore/100)} transform="rotate(-90 47 47)"/>}</Svg><View style={s.ringCopy}><Text testID="home-readiness" style={s.percent}>{displayScore === null ? '—' : `${Math.round(displayScore)}%`}</Text><Text style={[s.ringLabel,{fontSize:Math.max(9,11*scale)}]}>{scoreLabel}</Text></View></View></View>
                <View style={s.divider}>{vector({uri:'/assets/home/divider.svg'},337,.5)}</View>
                <View style={s.trendRow}>{typeof data?.readinessChangeThisMonth === 'number' && <Text testID="home-readiness-trend" style={s.trend}>{`${data.readinessChangeThisMonth > 0 ? '+' : ''}${data.readinessChangeThisMonth}% readiness this month`}</Text>}<Text style={[s.status,{color:data?.onTrack === true ? '#65deaa' : '#a6a6a4'}]}>{data?.onTrack === true ? '↗ On track' : data?.onTrack === false ? 'Needs attention' : readiness !== null ? 'Baseline' : displayScore !== null ? 'CV analysed' : 'Pending'}</Text></View>
            </View>
            <View style={s.sectionHead}><Text style={s.sectionTitle}>Your command centre</Text><Pressable accessibilityRole="button" accessibilityLabel={expanded ? 'View fewer shortcuts' : 'View all shortcuts'} onPress={
                /** Handles the on press for this component. */
                 () => setExpanded(!expanded)} style={s.textButton}><Text style={s.link}>{expanded ? 'VIEW LESS' : 'VIEW ALL'}</Text><OpportunityIcon name="arrow" size={16*scale}/></Pressable></View>
            <View style={s.grid}>{shortcuts.map(
                /** Renders a item entry from its data. */
                 item => <Pressable key={item.title} accessibilityRole="button" accessibilityLabel={item.title} onPress={
                /** Handles the on press for this component. */
                 () => { if(item.route==='jobs')model.opportunityModel.reset();model.go(item.route); }} style={s.shortcut}><Image source={item.icon} style={s.shortcutIcon} resizeMode="contain"/><View style={s.shortcutCopy}><Text style={s.shortcutTitle}>{item.title}</Text><Text style={s.shortcutMeta}>{item.copy}</Text></View></Pressable>)}</View>
            <View style={s.sectionHead}><Text style={s.sectionTitle}>Next mission</Text><Pressable accessibilityRole="button" accessibilityLabel="View missions" onPress={
                /** Opens the learning screen. */
                 () => model.go('learning')} style={s.textButton}><Text style={s.link}>VIEW MISSIONS</Text><OpportunityIcon name="arrow" size={16*scale}/></Pressable></View>
            <Pressable accessibilityRole="button" accessibilityLabel={next ? `Continue mission: ${next}` : 'Review completed missions'} onPress={openMission} style={s.mission}><MissionLandscape/>
                <View style={s.missionBottom}><View style={s.missionText}><Text style={s.link}>{next ? `MISSION ${missionNumber} / SKILLS` : topics.length ? 'ROADMAP COMPLETE' : 'NO MISSIONS YET'}</Text><Text testID="home-next-mission" style={s.missionTitle}>{next || (topics.length ? 'All missions complete' : 'Learning recommendations pending')}</Text></View><View style={s.tasks}><Text testID="home-task-count" style={s.tasksText}>{next ? `${done}/${total} TASKS` : `${model.completed.filter(
                    /** Selects the item that match the current conditions. */
                     topic=>topics.includes(topic)).length}/${topics.length} DONE`}</Text></View></View>
                <View accessibilityRole="progressbar" accessibilityLabel="Next mission progress" accessibilityValue={{min:0,max:100,now:total ? Math.round(done/total*100) : 0}} style={s.progress}><View testID="home-mission-progress" style={[s.progressFill,{width:`${total ? done/total*100 : 0}%`}]}/></View>
            </Pressable>
        </ScrollView>
        <Modal visible={expanded} transparent animationType="fade" onRequestClose={
            /** Handles the on request close for this component. */
             () => setExpanded(false)}><View style={s.scrim}><View style={s.dialog}><Text style={s.sectionTitle}>Your command centre</Text><ScrollView style={{maxHeight:height*.65}} contentContainerStyle={{gap:10,paddingVertical:16}}>{[...shortcuts.map(
            /** Transforms each item into the required display or state value. */
             item=>({title:item.title,route:item.route})),{title:'Campaign Medals',route:'achievements' as Screen},{title:'Ask MIRA',route:'mira' as Screen},{title:'Upload Transcript',route:'uploadTranscript' as Screen}].map(
            /** Renders a item entry from its data. */
             item=><Pressable key={item.title} accessibilityRole="button" accessibilityLabel={item.title} onPress={
            /** Handles the on press for this component. */
             ()=>{setExpanded(false);model.go(item.route);}} style={[s.extra,{width:'100%'}]}><Text style={s.shortcutTitle}>{item.title}</Text><OpportunityIcon name="arrow" size={16*scale}/></Pressable>)}</ScrollView><Pressable accessibilityRole="button" accessibilityLabel="Close command centre" onPress={
            /** Handles the on press for this component. */
             ()=>setExpanded(false)} style={s.close}><Text style={s.shortcutTitle}>Close</Text></Pressable></View></View></Modal>
        <Modal visible={!!notice} transparent onRequestClose={
            /** Displays feedback for the selected action. */
             () => setNotice('')}><View style={s.scrim}><View style={s.dialog}><Text style={s.dialogText}>{notice}</Text><Pressable accessibilityRole="button" onPress={
            /** Displays feedback for the selected action. */
             () => setNotice('')} style={s.close}><Text style={s.shortcutTitle}>Close</Text></Pressable></View></View></Modal>
    </View></View>;
}
const base = StyleSheet.create({
    safe:{flex:1,backgroundColor:'#000'},canvas:{flex:1,width:'100%',maxWidth:480,alignSelf:'center',overflow:'hidden'},landscape:{position:'absolute',top:.6,left:0,width:412,height:232},content:{paddingHorizontal:22,flexGrow:1},greeting:{flexDirection:'row',alignItems:'center',paddingHorizontal:11,gap:8},welcomeCopy:{flex:1,alignItems:'center',zIndex:2},nameRow:{flexDirection:'row',alignItems:'center',justifyContent:'center',gap:8},wave:{width:34,height:34},welcome:{fontSize:32,fontWeight:'700',color:'#fff'},name:{fontSize:32,fontWeight:'700',color:'#d4af53'},subtitle:{fontSize:13,fontWeight:'700',color:'#fff',marginTop:8,marginLeft:11,marginBottom:16},readinessCard:{minHeight:210,paddingHorizontal:22,paddingTop:18,paddingBottom:12,borderRadius:10,borderWidth:1,borderColor:'#353e44',backgroundColor:'rgba(6,15,20,.85)'},eyebrow:{fontSize:13,fontWeight:'700',color:'#d4af53'},readinessRow:{flexDirection:'row',alignItems:'center',gap:14,marginTop:14},readinessCopy:{flex:1},future:{fontSize:20,lineHeight:24,fontWeight:'700',color:'#fff'},stats:{fontSize:13,lineHeight:16,color:'#a6a6a4',marginTop:14},ring:{width:116,height:116,marginRight:0},ringCopy:{position:'absolute',top:0,right:0,bottom:0,left:0,justifyContent:'center',alignItems:'center'},percent:{fontSize:30,fontWeight:'700',color:'#fff'},ringLabel:{fontSize:10,color:'#a6a6a4',marginTop:3},divider:{height:.5,marginTop:15,marginLeft:-8,overflow:'hidden'},trendRow:{flexDirection:'row',alignItems:'center',justifyContent:'space-between',gap:8,marginTop:10},trend:{fontSize:12,color:'#a6a6a4',flexShrink:1},status:{fontSize:12,fontWeight:'700'},sectionHead:{minHeight:44,flexDirection:'row',alignItems:'center',justifyContent:'space-between',gap:12},sectionTitle:{fontSize:13,fontWeight:'700',color:'#fff'},textButton:{minHeight:44,flexDirection:'row',alignItems:'center',gap:6},link:{fontSize:10,fontWeight:'700',color:'#d4af53'},grid:{flexDirection:'row',flexWrap:'wrap',gap:8},shortcut:{width:'48.8%',minHeight:70,padding:8,borderRadius:10,borderWidth:1,borderColor:'#353e44',backgroundColor:'rgba(6,15,20,.85)',flexDirection:'row',alignItems:'center',gap:5},shortcutIcon:{width:43,height:43},shortcutCopy:{flex:1},shortcutTitle:{fontSize:11,fontWeight:'700',color:'#fff'},shortcutMeta:{fontSize:11,lineHeight:13,color:'#a6a6a4',marginTop:7},extra:{width:'48.8%',minHeight:50,padding:12,borderRadius:10,borderWidth:1,borderColor:'#353e44',backgroundColor:'#060f14',flexDirection:'row',alignItems:'center',justifyContent:'space-between'},mission:{minHeight:124,borderRadius:10,overflow:'hidden',borderWidth:1,borderColor:'#353e44',padding:10,justifyContent:'flex-end'},missionBottom:{flexDirection:'row',gap:10,alignItems:'center'},missionText:{flex:1},missionTitle:{fontSize:11,fontWeight:'700',color:'#fff',marginTop:3},tasks:{backgroundColor:'#3c2e1c',paddingHorizontal:10,paddingVertical:3,borderRadius:5},tasksText:{fontSize:12,color:'#c6a572'},progress:{height:6,backgroundColor:'#a6a6a4',borderRadius:50,overflow:'hidden',marginTop:8},progressFill:{height:'100%',backgroundColor:'#d4af53',borderRadius:50},scrim:{flex:1,backgroundColor:'rgba(0,0,0,.7)',padding:24,alignItems:'center',justifyContent:'center'},dialog:{width:'100%',maxWidth:420,backgroundColor:'#0b151c',padding:20,borderRadius:16,borderWidth:1,borderColor:'#353e44'},dialogText:{fontSize:14,lineHeight:22,color:'#fff',marginBottom:20},close:{minHeight:44,backgroundColor:'#8c6c27',borderRadius:8,alignItems:'center',justifyContent:'center'},
});
