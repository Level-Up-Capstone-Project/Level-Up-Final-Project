import type {BundledAsset} from '../platform/assets';
/** Displays CV assessment results, findings and the full feedback report. */
import VignettePair from '../components/VignettePair';
import ScreenHeader, {screenHeaderTop} from '../components/ScreenHeader';
import { useEffect, useState } from 'react';
import { Platform, BackHandler, Image, Modal, Pressable, ScrollView, StyleSheet, View } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import Svg, { Circle, Defs, LinearGradient, Path, Rect, Stop } from 'react-native-svg';
import DocumentResults from '../analysis/DocumentResults';
import CvFeedbackReport from '../analysis/CvFeedbackReport';
import TargetIcon from '../components/TargetIcon';
import FigmaIcon from '../components/FigmaIcon';
import Text from '../components/Text';
import ChevronLeft from '../components/ChevronLeft';
import OpportunityIcon from '../opportunities/OpportunityIcon';
import LocalVector from '../onboarding/LocalVector';
import useResponsiveStyles from '../layout/useResponsiveStyles';
import type { AppModel } from '../navigation/useAppModel';

/** Displays CV assessment results, findings and the full feedback report. */
export default function CvAnalysisReportScreen({model}:{model:AppModel}) {
    const s=useResponsiveStyles(base),insets=useSafeAreaInsets(),scale=Math.min(model.width,480)/408.393;
    const result=model.cvAssessment,score=result?.score??null;
    const strengths=result?.strengths??[];
    const improvements=result?.improvements??result?.findings.map(
        /** Transforms each finding into the required display or state value. */
         (description,index)=>({title:`Improvement ${index+1}`,description,priority:undefined}))??[];
    const [detail,setDetail]=useState<{title:string;description:string}|null>(null);
    const linkedTopic=result?.learningTopic;
    const canComplete=!!linkedTopic&&model.learningGroups.some(
        /** Checks the group against the current selection or criteria. */
         group=>group.topics.includes(linkedTopic));
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
    return <View style={s.safe}><View testID="cv-report-screen" style={s.canvas}>
        <View pointerEvents="none" style={StyleSheet.absoluteFill}>
            <Image testID="cv-report-landscape" source={{uri:'/assets/cv-report/landscape.png'}} resizeMode="stretch" style={{position:'absolute',left:0,top:.6*scale,width:407.98*scale,height:229.67*scale}}/>
            <VignettePair height={885.372*scale}/>
            <View style={{position:'absolute',top:.22*scale,left:.22*scale,opacity:.35}}>{vector({uri:'/assets/home/upper.svg'},186.551)}</View>
            <View style={{position:'absolute',bottom:0,left:.22*scale,opacity:.35}}>{vector({uri:'/assets/home/lower.svg'},310.065)}</View>
        </View>
        <ScrollView testID="cv-report-content" contentContainerStyle={[s.content,{paddingTop:screenHeaderTop(scale,insets.top),paddingBottom:Math.max(14*scale,insets.bottom+8)}]}>
            <ScreenHeader title="AI REPORT" onBack={
                /** Opens the analysis screen. */
                 ()=>model.goBack('analysis')} backLabel="Back to document analysis" scale={scale}/>
            <Text style={s.eyebrow}>YOUR PERSONALISED INTEL</Text>
            <View style={[s.card,s.scoreCard]}><View accessibilityLabel={score===null?'CV strength pending':`CV strength ${score}%`} style={s.ring}><Svg width={94*scale} height={94*scale} viewBox="0 0 94 94"><Circle cx={47} cy={47} r={44} fill="none" stroke="#d1d1d6" strokeWidth={6}/>{score!==null&&<Circle cx={47} cy={47} r={44} fill="none" stroke="#6de0b3" strokeWidth={6} strokeDasharray={`${2*Math.PI*44*score/100} ${2*Math.PI*44}`} transform="rotate(-90 47 47)"/>}</Svg><View style={s.ringText}><Text testID="cv-report-score" style={s.score}>{score===null?'—':score+'%'}</Text><Text style={s.ringCaption}>CV Strength</Text></View></View><View style={s.copy}><Text style={s.goldHeading}>{result?.headline||'ANALYSIS PENDING'}</Text><Text style={s.scoreDescription}>{result?.summary||'Your personalised report will appear after your documents are analysed.'}</Text></View></View>
            <DocumentResults atsScore={result?.atsScore} formattingScore={result?.formattingScore} transcript={model.transcriptAssessment} onTranscript={()=>model.go('uploadTranscript')}/>
            <View style={s.reportSection}><Text style={s.itemTitle}>What you’re doing well</Text><Text testID="report-strength-count" style={s.goldHeading}>{strengths.length} ITEMS</Text></View>
            {!strengths.length&&<Text style={s.empty}>{result?'No strengths have been supplied in this report.':'Strengths will appear when analysis results are available.'}</Text>}
            {strengths.map(
                /** Renders a strength entry from its data. */
                 (item,index)=><Pressable key={index} accessibilityRole="button" accessibilityLabel={'View strength: '+item.title} onPress={
                /** Opens the selected report detail. */
                 ()=>setDetail(item)} style={s.reportItem}><View style={{width:43*scale,alignItems:'center'}}><FigmaIcon name="check" size={24*scale} color="#6de0b3"/></View><View style={s.copy}><Text style={s.itemTitle}>{item.title}</Text><Text style={s.itemDescription}>{item.description}</Text></View><View style={{transform:[{rotate:'180deg'}]}}><ChevronLeft size={20*scale} color="#d4af53"/></View></Pressable>)}
            <View style={s.reportSection}><Text style={s.itemTitle}>Top Improvements</Text><Text testID="report-improvement-count" style={s.goldHeading}>{improvements.length} ACTIONS</Text></View>
            {!improvements.length&&<Text style={s.empty}>{result?'No improvement actions have been supplied in this report.':'Improvement actions will appear when analysis results are available.'}</Text>}
            {improvements.map(
                /** Renders a improvement entry from its data. */
                 (item,index)=><View key={index} style={s.reportItem}><TargetIcon testID={'report-target-'+index} size={45*scale}/><View style={s.copy}><Text style={s.itemTitle}>{item.title}</Text><Text style={s.itemDescription}>{item.description}</Text></View>{item.priority&&<View style={[s.priority,{backgroundColor:item.priority==='high'?'#3e2724':item.priority==='medium'?'#3c2e1c':'#163a31'}]}><Text style={[s.priorityText,{color:item.priority==='high'?'#f6a28d':item.priority==='medium'?'#c6a572':'#6de0b3'}]}>{item.priority.toUpperCase()}</Text></View>}</View>)}
            <CvFeedbackReport feedback={result?.feedback??(result?.findings.length?{sections:[{heading:'Analysis Findings',bullets:result.findings}]}:undefined)}/>
            <View style={{flex:1,minHeight:0}}/>
            <Pressable accessibilityRole="button" accessibilityLabel={canComplete?'Mark Topic Complete':'Back to CV Analysis'} onPress={
                /** Opens the complete screen. */
                 ()=>{if(canComplete&&linkedTopic){model.setTopic(linkedTopic);model.setCompleted(
                /** Builds the next completed value from its previous state. */
                 old=>Array.from(new Set([...old,linkedTopic])));model.go('complete');}else model.go('analysis');}} style={s.continue}><Text style={s.continueText}>{canComplete?'Mark Topic Complete':'Back to CV Analysis'}</Text><OpportunityIcon name="arrow" size={24*scale} color="#000"/></Pressable>
        </ScrollView>
        <Modal visible={detail!==null} transparent animationType="fade" onRequestClose={
            /** Closes the selected dialog or detail view. */
             ()=>setDetail(null)}><View style={s.backdrop}><View accessibilityViewIsModal style={s.dialog}><ScrollView contentContainerStyle={{padding:20}}><Text accessibilityRole="header" style={s.dialogTitle}>{detail?.title}</Text><Text selectable style={s.meta}>{detail?.description}</Text><Pressable accessibilityRole="button" accessibilityLabel="Close strength details" onPress={
            /** Closes the selected dialog or detail view. */
             ()=>setDetail(null)} style={[s.continue,{marginTop:20}]}><Text style={s.continueText}>Close</Text></Pressable></ScrollView></View></View></Modal>
    </View></View>;
}
const base=StyleSheet.create({
    safe:{flex:1,backgroundColor:'#000'},canvas:{flex:1,width:'100%',maxWidth:480,alignSelf:'center',overflow:'hidden'},content:{flexGrow:1,paddingHorizontal:23},header:{flexDirection:'row',alignItems:'center',marginLeft:-9,minHeight:44},iconButton:{minWidth:44,minHeight:44,alignItems:'center',justifyContent:'center'},title:{fontSize:30,fontWeight:'600',color:'#fff',letterSpacing:-.8},card:{borderWidth:1,borderColor:'#353e44',borderRadius:10,backgroundColor:'rgba(6,15,20,.78)'},roleCard:{marginTop:113,minHeight:66,padding:11,flexDirection:'row',alignItems:'center',gap:10},roleIcon:{width:38,height:38,overflow:'hidden'},copy:{flex:1},label:{fontSize:12,lineHeight:16,color:'#fff'},small:{fontSize:10,lineHeight:16,color:'#d1d1d6',marginTop:4},heading:{fontSize:12,lineHeight:16,fontWeight:'700',color:'#fff'},goldHeading:{fontSize:12,lineHeight:16,fontWeight:'700',color:'#d4af53'},tabs:{flexDirection:'row',gap:15,marginTop:17,marginBottom:17},tab:{minHeight:46,minWidth:96,paddingHorizontal:10,justifyContent:'center',alignItems:'center',backgroundColor:'#090e11',borderWidth:1,borderColor:'#d4af53',borderRadius:10},selectedTab:{backgroundColor:'#d4af53'},selectedTabText:{fontSize:13,lineHeight:16,fontWeight:'700',color:'#000'},scoreCard:{minHeight:135,flexDirection:'row',alignItems:'center',gap:32,paddingHorizontal:22,paddingVertical:18},ring:{width:94,height:94,alignItems:'center',justifyContent:'center'},ringText:{position:'absolute',alignItems:'center'},score:{fontSize:20,lineHeight:24,color:'#fff',fontWeight:'700'},ringCaption:{fontSize:10,lineHeight:16,color:'#d1d1d6'},scoreDescription:{fontSize:10,lineHeight:22,color:'#d1d1d6',marginTop:5},sectionRow:{flexDirection:'row',alignItems:'center',justifyContent:'space-between',minHeight:37},link:{minHeight:44,flexDirection:'row',alignItems:'center',gap:5},fileCard:{minHeight:66,flexDirection:'row',alignItems:'center',gap:10,padding:12},fileIcon:{width:40,height:39},status:{paddingHorizontal:5,paddingVertical:2,borderRadius:5,backgroundColor:'rgba(33,96,80,.7)',maxWidth:90},statusText:{fontSize:12,lineHeight:16,color:'#6de0b3'},reportCard:{minHeight:74,flexDirection:'row',alignItems:'center',gap:10,paddingHorizontal:9,borderRadius:10,borderWidth:1,borderColor:'#765728',backgroundColor:'#252112'},aiIcon:{width:63,height:63},continue:{minHeight:54,borderRadius:10,borderWidth:1,borderColor:'#ba7e27',backgroundColor:'#c4872d',flexDirection:'row',alignItems:'center',justifyContent:'center',gap:20,padding:10},continueText:{fontFamily:'PoppinsMedium',fontSize:16,lineHeight:24,color:'#000'},backdrop:{flex:1,backgroundColor:'rgba(0,0,0,.8)',justifyContent:'center',padding:24},dialog:{width:'100%',maxWidth:440,maxHeight:'80%',alignSelf:'center',backgroundColor:'#060f14',borderColor:'#353e44',borderWidth:1,borderRadius:12},dialogTitle:{fontSize:20,fontWeight:'700',color:'#fff'},meta:{fontSize:13,lineHeight:20,color:'#d1d1d6',marginTop:16},roleOption:{minHeight:48,flexDirection:'row',justifyContent:'space-between',alignItems:'center',borderBottomWidth:1,borderBottomColor:'#353e44',paddingVertical:12},
eyebrow:{fontSize:13,lineHeight:16,fontWeight:'700',color:'#d4af53',marginTop:77,marginLeft:3,marginBottom:10},reportSection:{flexDirection:'row',alignItems:'center',justifyContent:'space-between',gap:8,marginTop:10,marginBottom:10},reportItem:{minHeight:80,flexDirection:'row',alignItems:'center',gap:14,borderWidth:1,borderColor:'#353e44',borderRadius:10,backgroundColor:'rgba(6,15,20,.85)',paddingHorizontal:13,paddingVertical:14,marginBottom:17},itemTitle:{fontSize:13,lineHeight:16,fontWeight:'700',color:'#fff'},itemDescription:{fontSize:13,lineHeight:18,color:'#a6a6a4',marginTop:8},targetIcon:{width:45,height:45},priority:{borderRadius:5,paddingHorizontal:10,paddingVertical:2},priorityText:{fontSize:12,lineHeight:16},empty:{fontSize:13,lineHeight:20,color:'#a6a6a4',marginBottom:17,paddingVertical:20,paddingHorizontal:14,borderWidth:1,borderColor:'#353e44',borderRadius:10,backgroundColor:'rgba(6,15,20,.85)'},
});
