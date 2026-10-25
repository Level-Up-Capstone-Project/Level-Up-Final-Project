/** Displays the selected CV, target role, assessment summary and report links. */
import VignettePair from '../components/VignettePair';
import ScreenHeader, {screenHeaderTop} from '../components/ScreenHeader';
import { useEffect, useState } from 'react';
import { BackHandler, Image, Linking, Modal, Platform, Pressable, ScrollView, StyleSheet, View } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import Svg, { Circle } from 'react-native-svg';
import Text from '../components/Text';
import ChevronRight from '../components/ChevronRight';
import OpportunityIcon from '../opportunities/OpportunityIcon';
import useResponsiveStyles from '../layout/useResponsiveStyles';
import type { AppModel } from '../navigation/useAppModel';
import DocumentResults from '../analysis/DocumentResults';
import FileTypeIcon from '../upload/FileTypeIcon';

/** Displays the selected CV, target role, assessment summary and report links. */
export default function CvAnalysisScreen({model}:{model:AppModel}) {
    const roles=model.roles;
    const s=useResponsiveStyles(base),insets=useSafeAreaInsets(),scale=Math.min(model.width,480)/408.393;
    const [panel,setPanel]=useState<'role'|'file'|'report'|null>(null),[fileError,setFileError]=useState('');
    const document=model.cvDocument,result=model.cvAssessment;
    const transcript=model.transcriptDocument, transcriptResult=model.transcriptAssessment;
    const openTranscript=async()=>{if(!transcript)return;try{await Linking.openURL(transcript.uri);}catch{setFileError('The transcript could not be opened on this device. Upload it again.');}};
    const transcriptStatus=model.transcriptAnalysis.status==='processing'?'ANALYSING':model.transcriptAnalysis.status==='failed'?'FAILED':transcriptResult?'ANALYSED':transcript?'UPLOADED':'NO FILE';
    const transcriptMetadata=transcript?[transcriptResult?'Last analysed '+new Date(transcriptResult.analysedAt).toLocaleDateString('en-ZA'):'Not analysed yet',transcript.size===undefined?'':(transcript.size/1024/1024).toFixed(2)+' MB','PDF'].filter(Boolean).join(' | '):transcriptResult?['Last analysed '+new Date(transcriptResult.analysedAt).toLocaleDateString('en-ZA'),transcriptResult.fileSize===undefined?'':(transcriptResult.fileSize/1024/1024).toFixed(2)+' MB','PDF'].filter(Boolean).join(' | '):'Upload a transcript to see its details';
    const score=result?.score??null;
    useEffect(
        /** Registers the hardware back action and its cleanup. */
         ()=>{
    /** Handles the device back action for this screen. */
    if (Platform.OS === 'web') return;
    const back=BackHandler.addEventListener('hardwareBackPress', ()=>{model.goBack('home');return true;});
    /** Removes the registered listeners when the effect ends. */
    return  ()=>back.remove();},[model.goBack]);
    /** Opens the selected CV with a compatible application on the device. */
    const openFile= async()=>{if(!document)return;try {const uri=document.uri;await Linking.openURL(uri);}catch{setFileError('The file could not be opened on this device. You can upload it again.');}};
    const chevron=<ChevronRight size={20*scale} color="#d4af53"/>;
    const status=model.cvAnalysis.status==='processing'?'ANALYSING':model.cvAnalysis.status==='failed'?'FAILED':result?'ANALYSED':document?'UPLOADED':'NO FILE';
    const metadata=document?[result?'Last analysed '+new Date(result.analysedAt).toLocaleDateString('en-ZA'):'Not analysed yet',document.size===undefined?'':(document.size/1024/1024).toFixed(2)+' MB',document.name.split('.').pop()?.toUpperCase()].filter(Boolean).join(' | '):'Upload a CV to see its details';
    return <View style={s.safe}><View testID="cv-analysis-screen" style={s.canvas}>
        <View pointerEvents="none" style={StyleSheet.absoluteFill}>
            <Image testID="cv-analysis-landscape" source={{uri:'/assets/cv-analysis/92fd0.png'}} resizeMode="cover" style={{position:'absolute',left:-38.78*scale,top:.22*scale,width:486*scale,height:274*scale}}/>
            <VignettePair height={885.372*scale}/>
        </View>
        <ScrollView testID="cv-analysis-content" contentContainerStyle={[s.content,{paddingTop:screenHeaderTop(scale,insets.top),paddingBottom:Math.max(41*scale,insets.bottom+8)}]}>
            <ScreenHeader title="DOCUMENT ANALYSIS" onBack={
                /** Opens the home screen. */
                 ()=>model.goBack('home')} backLabel="Back to previous page" scale={scale}/>
            <Pressable accessibilityRole="button" accessibilityLabel="Choose target role" onPress={
                /** Opens the role panel. */
                 ()=>setPanel('role')} style={[s.card,s.roleCard]}><View style={s.roleIcon}><Image source={{uri:'/assets/cv-analysis/b1e5a.png'}} resizeMode="stretch" style={{position:'absolute',width:61.77*scale,height:48.97*scale,left:-12.07*scale,top:-7.72*scale}}/></View><View style={s.copy}><Text style={s.label}>Target Role</Text><Text style={s.small}>{roles[model.role] || 'Set your target roles'}</Text></View>{chevron}</Pressable>
            <View style={s.tabs}><View accessibilityRole="tab" accessibilityState={{selected:true}} style={[s.tab,s.selectedTab]}><Text style={s.selectedTabText}>CV Analysis</Text></View><Pressable accessibilityRole="tab" accessibilityState={{selected:false}} accessibilityLabel="Skills Match" onPress={
                /** Opens the skills screen. */
                 ()=>model.go('skills')} style={s.tab}><Text style={s.label}>Skills Match</Text></Pressable></View>
            <View style={[s.card,s.scoreCard]}><View testID="cv-score-ring" accessibilityLabel={score===null?'CV strength pending':`CV strength ${score}%`} style={s.ring}><Svg width={94*scale} height={94*scale} viewBox="0 0 94 94"><Circle cx={47} cy={47} r={44} fill="none" stroke="#d1d1d6" strokeWidth={6}/>{score!==null&&<Circle cx={47} cy={47} r={44} fill="none" stroke="#6de0b3" strokeWidth={6} strokeDasharray={`${2*Math.PI*44*score/100} ${2*Math.PI*44}`} transform="rotate(-90 47 47)"/>}</Svg><View style={s.ringText}><Text testID="cv-score" style={s.score}>{score===null?'—':score+'%'}</Text><Text style={s.ringCaption}>CV Strength</Text></View></View><View style={s.copy}><Text style={s.goldHeading}>YOUR CV SCORE</Text><Text style={[s.heading,{marginTop:8*scale}]}>{result?.headline||'Analysis pending'}</Text><Text style={s.scoreDescription}>{result?.summary||(document?'Your CV score will appear after analysis.':'Upload your CV to get started.')}</Text></View></View>
            <DocumentResults atsScore={result?.atsScore} formattingScore={result?.formattingScore} transcript={model.transcriptAssessment} onTranscript={() => model.go('uploadTranscript')}/>
            <View style={s.sectionRow}><Text style={s.heading}>Your CV Snapshot</Text><Pressable accessibilityRole="button" accessibilityLabel={document?'View file':'Upload CV'} onPress={
                /** Opens the CV file or upload flow. */
                 ()=>document?setPanel('file'):model.go('upload')} hitSlop={8} style={s.link}><Text style={s.goldHeading}>{document?'VIEW FILE':'UPLOAD CV'}</Text><OpportunityIcon name="arrow" size={16*scale}/></Pressable></View>
            <Pressable accessibilityRole="button" accessibilityLabel="CV snapshot" onPress={
                /** Opens the CV file or upload flow. */
                 ()=>document?setPanel('file'):model.go('upload')} style={[s.card,s.fileCard]}><FileTypeIcon filename={document?.name ?? result?.fileName ?? ''} size={49*scale}/><View style={s.copy}><Text numberOfLines={2} style={s.heading}>{document?.name||result?.fileName||(result?'Saved CV analysis':'No CV uploaded')}</Text><Text style={s.small}>{result&&!document?'Last analysed '+new Date(result.analysedAt).toLocaleDateString('en-ZA'):metadata}</Text></View><View style={s.status}><Text style={s.statusText}>{status}</Text></View></Pressable>
            <View style={s.sectionRow}><Text style={s.heading}>Your Transcript Snapshot</Text><Pressable accessibilityRole="button" accessibilityLabel={transcript?'View transcript file':'Upload transcript'} onPress={()=>transcript?void openTranscript():model.go('uploadTranscript')} hitSlop={8} style={s.link}><Text style={s.goldHeading}>{transcript?'VIEW FILE':'UPLOAD TRANSCRIPT'}</Text><OpportunityIcon name="arrow" size={16*scale}/></Pressable></View>
            <Pressable accessibilityRole="button" accessibilityLabel="Transcript snapshot" onPress={()=>transcript?void openTranscript():model.go('uploadTranscript')} style={[s.card,s.fileCard]}><FileTypeIcon filename={transcript?.name ?? transcriptResult?.fileName ?? ''} size={49*scale}/><View style={s.copy}><Text numberOfLines={2} style={s.heading}>{transcript?.name||transcriptResult?.fileName||'No transcript uploaded'}</Text><Text style={s.small}>{transcriptMetadata}</Text></View><View style={s.status}><Text style={s.statusText}>{transcriptStatus}</Text></View></Pressable>
            {!!fileError&&<Text accessibilityRole="alert" style={s.small}>{fileError}</Text>}
            <View style={[s.sectionRow,{marginTop:14*scale,minHeight:28*scale}]}><Text style={s.heading}>AI Report</Text><Pressable accessibilityRole="button" accessibilityLabel="Full report" onPress={
                /** Opens the cv report screen. */
                 ()=>model.go('cvReport')} hitSlop={8} style={s.link}><Text style={s.goldHeading}>FULL REPORT</Text><OpportunityIcon name="arrow" size={16*scale}/></Pressable></View>
            <Pressable accessibilityRole="button" accessibilityLabel="AI Report" onPress={
                /** Opens the cv report screen. */
                 ()=>model.go('cvReport')} style={s.reportCard}><Image source={{uri:'/assets/cv-analysis/8701f.png'}} resizeMode="cover" style={s.aiIcon}/><View style={s.copy}><Text style={s.heading}>AI Report</Text><Text style={s.small}>{result?`CV${transcriptResult?' & transcript':''} analysis results`:'Findings will appear after analysis.'}</Text></View>{chevron}</Pressable>
            <View style={{flex:1,minHeight:51*scale}}/>
            <Pressable accessibilityRole="button" accessibilityLabel="Explore My Skills Gaps" onPress={
                /** Opens the skills screen. */
                 ()=>model.go('skills')} style={s.continue}><Text style={s.continueText}>Explore My Skills Gaps</Text><OpportunityIcon name="arrow" size={24*scale} color="#000"/></Pressable>
        </ScrollView>
        <Modal visible={panel!==null} transparent animationType="fade" onRequestClose={
            /** Handles the on request close for this component. */
             ()=>setPanel(null)}><View style={s.backdrop}><View accessibilityViewIsModal style={s.dialog}><ScrollView contentContainerStyle={{padding:20}}><Text accessibilityRole="header" style={s.dialogTitle}>{panel==='role'?'Target Role':panel==='file'?'Your CV Snapshot':'AI Report'}</Text>{panel==='role'?<>{!roles.length&&<Pressable accessibilityRole="button" onPress={
            /** Opens the profile settings screen. */
             ()=>{setPanel(null);model.go('profileSettings');}} style={s.roleOption}><Text style={s.goldHeading}>Add target roles in Profile Settings</Text></Pressable>}{roles.map(
            /** Renders a role entry from its data. */
             (role,index)=><Pressable key={role} accessibilityRole="radio" accessibilityState={{checked:index===model.role}} onPress={
            /** Handles the on press for this component. */
             ()=>{model.setRole(index);setPanel(null);}} style={s.roleOption}><Text style={s.label}>{role}</Text><Text style={s.goldHeading}>{model.role===index?'✓':''}</Text></Pressable>)}</>:panel==='file'?<><Text style={[s.label,{marginTop:16}]}>{document?.name}</Text><Text style={s.meta}>{metadata}</Text><Pressable accessibilityRole="button" accessibilityLabel="Open uploaded file" onPress={openFile} style={[s.continue,{marginTop:20}]}><Text style={s.continueText}>Open File</Text></Pressable>{!!fileError&&<Text accessibilityRole="alert" style={s.meta}>{fileError}</Text>}<Pressable accessibilityRole="button" accessibilityLabel="Replace CV" onPress={
            /** Opens the upload screen. */
             ()=>{setPanel(null);model.go('upload');}} style={s.roleOption}><Text style={s.goldHeading}>Replace CV</Text></Pressable></>:<><Text style={s.meta}>{result?.summary||'Your AI report will be available when your CV has been analysed by the connected analysis service.'}</Text>{result?.findings.map(
            /** Renders a finding entry from its data. */
             (finding,index)=><Text key={index} style={s.meta}>• {finding}</Text>)}</>}<Pressable accessibilityRole="button" accessibilityLabel="Close dialog" onPress={
            /** Handles the on press for this component. */
             ()=>{setPanel(null);setFileError('');}} style={[s.continue,{marginTop:20}]}><Text style={s.continueText}>Close</Text></Pressable></ScrollView></View></View></Modal>
    </View></View>;
}
const base=StyleSheet.create({
    safe:{flex:1,backgroundColor:'#000'},canvas:{flex:1,width:'100%',maxWidth:480,alignSelf:'center',overflow:'hidden'},content:{flexGrow:1,paddingHorizontal:23},header:{flexDirection:'row',alignItems:'center',marginLeft:-7,minHeight:44},iconButton:{minWidth:44,minHeight:44,alignItems:'center',justifyContent:'center'},title:{fontSize:30,fontWeight:'700',color:'#fff',letterSpacing:-.8},card:{borderWidth:1,borderColor:'#353e44',borderRadius:10,backgroundColor:'rgba(6,15,20,.78)'},roleCard:{marginTop:145,minHeight:66,padding:11,flexDirection:'row',alignItems:'center',gap:10},roleIcon:{width:38,height:38,overflow:'hidden'},copy:{flex:1},label:{fontSize:12,lineHeight:16,color:'#fff'},small:{fontSize:10,lineHeight:16,color:'#d1d1d6',marginTop:4},heading:{fontSize:12,lineHeight:16,fontWeight:'700',color:'#fff'},goldHeading:{fontSize:12,lineHeight:16,fontWeight:'700',color:'#d4af53'},tabs:{flexDirection:'row',gap:15,marginTop:17,marginBottom:17},tab:{minHeight:46,minWidth:96,paddingHorizontal:10,justifyContent:'center',alignItems:'center',backgroundColor:'#090e11',borderWidth:1,borderColor:'#d4af53',borderRadius:10},selectedTab:{backgroundColor:'#d4af53'},selectedTabText:{fontSize:13,lineHeight:16,fontWeight:'700',color:'#000'},scoreCard:{minHeight:135,flexDirection:'row',alignItems:'center',gap:32,paddingHorizontal:22,paddingVertical:18},ring:{width:94,height:94,alignItems:'center',justifyContent:'center'},ringText:{position:'absolute',alignItems:'center'},score:{fontSize:20,lineHeight:24,color:'#fff',fontWeight:'700'},ringCaption:{fontSize:10,lineHeight:16,color:'#d1d1d6'},scoreDescription:{fontSize:10,lineHeight:22,color:'#d1d1d6',marginTop:5},sectionRow:{flexDirection:'row',alignItems:'center',justifyContent:'space-between',minHeight:37},link:{minHeight:28,flexDirection:'row',alignItems:'center',gap:5},fileCard:{minHeight:66,flexDirection:'row',alignItems:'center',gap:10,padding:12},fileIcon:{width:40,height:39},status:{paddingHorizontal:5,paddingVertical:2,borderRadius:5,backgroundColor:'rgba(33,96,80,.7)',maxWidth:90},statusText:{fontSize:12,lineHeight:16,color:'#6de0b3'},reportCard:{minHeight:74,flexDirection:'row',alignItems:'center',gap:10,paddingHorizontal:9,borderRadius:10,borderWidth:1,borderColor:'#765728',backgroundColor:'#252112'},aiIcon:{width:63,height:63},continue:{marginLeft:-5,marginRight:5,minHeight:54,borderRadius:10,borderWidth:1,borderColor:'#ba7e27',backgroundColor:'#c4872d',flexDirection:'row',alignItems:'center',justifyContent:'center',gap:20,padding:10},continueText:{fontFamily:'PoppinsMedium',fontSize:16,lineHeight:24,color:'#000'},backdrop:{flex:1,backgroundColor:'rgba(0,0,0,.8)',justifyContent:'center',padding:24},dialog:{width:'100%',maxWidth:440,maxHeight:'80%',alignSelf:'center',backgroundColor:'#060f14',borderColor:'#353e44',borderWidth:1,borderRadius:12},dialogTitle:{fontSize:20,fontWeight:'700',color:'#fff'},meta:{fontSize:13,lineHeight:20,color:'#d1d1d6',marginTop:16},roleOption:{minHeight:48,flexDirection:'row',justifyContent:'space-between',alignItems:'center',borderBottomWidth:1,borderBottomColor:'#353e44',paddingVertical:12},
});
