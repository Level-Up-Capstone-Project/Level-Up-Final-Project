import type {BundledAsset} from '../platform/assets';
/** Displays a learning topic, its resources and completion checklist. */
import VignettePair from '../components/VignettePair';
import ScreenHeader, {screenHeaderTop} from '../components/ScreenHeader';
import { useEffect, useState } from 'react';
import { Platform, BackHandler, Image, Linking, Modal, Pressable, ScrollView, StyleSheet, View } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import Text from '../components/Text';
import ChevronLeft from '../components/ChevronLeft';
import OpportunityIcon from '../opportunities/OpportunityIcon';
import LocalVector from '../onboarding/LocalVector';
import useResponsiveStyles from '../layout/useResponsiveStyles';
import type { AppModel } from '../navigation/useAppModel';

/** Displays a learning topic, its resources and completion checklist. */
export default function MissionIntelScreen({model}:{model:AppModel}) {
    const s=useResponsiveStyles(base),insets=useSafeAreaInsets();
    const {learningGroups}=model;
    const scale=Math.min(model.width,480)/408.393;
    const resources=model.currentTopic?.resources ?? [];
    const mission=learningGroups.findIndex(
        /** Checks the group against the current selection or criteria. */
         group=>group.topics.includes(model.topic))+1;
    const [selected,setSelected]=useState<number|null>(null);
    const [resourceError,setResourceError]=useState('');
    const [completing,setCompleting]=useState(false);
    const saved=model.savedTopics.includes(model.topic);
    const openResource=async(url:string)=>{
        setResourceError('');
        if(!/^https?:\/\//i.test(url)){setResourceError('This resource does not have a valid link.');return;}
        try{await Linking.openURL(url);}
        catch{setResourceError('The resource could not be opened. Please try again.');}
    };
    useEffect(
        /** Registers the hardware back action and its cleanup. */
         ()=>{
    /** Handles the device back action for this screen. */
    if (Platform.OS === 'web') return;
    const back=BackHandler.addEventListener('hardwareBackPress', ()=>{model.goBack('learning');return true;});
    /** Removes the registered listeners when the effect ends. */
    return  ()=>back.remove();},[model.goBack]);
    /** Displays bundled vector artwork at the calculated screen scale. */
    const vector= (source:BundledAsset,height:number)=><View style={{width:408.8,height,transformOrigin:'top left',transform:[{scale}]}}><LocalVector source={source} width={408.8} height={height}/></View>;
    return <View style={s.safe}><View testID="mission-intel-screen" style={s.canvas}>
        <View pointerEvents="none" style={StyleSheet.absoluteFill}>
            <Image testID="mission-intel-landscape" source={{uri:'/assets/mission-intel/landscape.png'}} resizeMode="stretch" style={{position:'absolute',left:0,top:.6*scale,width:407.98*scale,height:229.67*scale}}/>
            <VignettePair height={885.372*scale}/>
            <View style={{position:'absolute',top:.22*scale,left:.22*scale,opacity:.35}}>{vector({uri:'/assets/home/upper.svg'},186.551)}</View>
            <View style={{position:'absolute',bottom:0,left:.22*scale,opacity:.35}}>{vector({uri:'/assets/home/lower.svg'},310.065)}</View>
        </View>
        <ScrollView testID="mission-intel-content" contentContainerStyle={[s.content,{paddingTop:screenHeaderTop(scale,insets.top),paddingBottom:Math.max(25*scale,insets.bottom+8)}]}>
            <ScreenHeader title="MISSION INTEL" onBack={
                /** Opens the learning screen. */
                 ()=>model.goBack('learning')} backLabel="Back to previous page" scale={scale}/>
            <View style={s.intro}><View style={s.row}><Text style={s.missionLabel}>{mission ? `MISSION ${mission}` : 'NO MISSION SELECTED'}</Text><Pressable accessibilityRole="button" accessibilityLabel={saved?'Unsave topic':'Save topic'} accessibilityState={{selected:saved}} disabled={!model.topic} onPress={model.toggleSavedTopic} style={s.iconButton}><OpportunityIcon name="bookmark" size={18*scale} filled={saved}/></Pressable></View><Text style={s.description}>{model.topic || 'Select a mission when learning recommendations are available.'}</Text></View>
            <View style={s.badges}><View style={[s.badge,s.format]}><Text style={[s.badgeText,{color:'#c6a572'}]}>{resources[0]?.resource.resourceType?.toUpperCase()||'LEARNING'}</Text></View><View style={[s.badge,s.duration]}><Text style={[s.badgeText,{color:'#629fc1'}]}>{resources[0]?.resource.estDuration?.toUpperCase()||'SELF-PACED'}</Text></View><View style={[s.badge,s.complexity]}><Text style={[s.badgeText,{color:'#6de0b3'}]}>{resources[0]?.resource.difficultyLevel?.toUpperCase()||'BEGINNER'}</Text></View></View>
            <View style={[s.row,s.sectionRow]}><Text style={s.heading}>Your resources</Text><Text style={s.count}>{resources.length} ITEMS</Text></View>
            {!resources.length&&<Text accessibilityRole="alert" style={s.empty}>No learning resources are linked to this mission yet.</Text>}
            {resources.map(
                /** Renders a resource entry from its data. */
                 (item,index)=><Pressable key={item.roadmapItemResourceID} accessibilityRole="button" accessibilityLabel={'Open resource: '+item.resource.resourceName} onPress={
                /** Selects the item to display in the detail view. */
                 ()=>setSelected(index)} style={s.resource}><View style={s.resourceCopy}><Text style={s.heading}>{item.resource.resourceName}</Text><Text style={s.meta}>{item.resource.provider} · {item.resource.resourceType}</Text><Text style={s.resourceDetails}>{item.resource.estDuration} · {item.resource.difficultyLevel} · {item.resource.costType||'Free'}</Text></View><View style={{transform:[{rotate:'180deg'}]}}><ChevronLeft size={20*scale} color="#d4af53"/></View></Pressable>)}
            <Text style={[s.heading,s.learnHeading]}>What you’ll learn</Text><View style={s.learnCard}>{resources.map(
                /** Renders a resource entry from its data. */
                 (item,index)=><Pressable key={item.roadmapItemResourceID} accessibilityRole="checkbox" accessibilityLabel={item.resource.resourceName} accessibilityState={{checked:!!model.checked[index],disabled:!!model.checked[index]}} disabled={!!model.checked[index]} onPress={
                /** Updates the selected resource checklist entry. */
                 ()=>model.setChecked(
                /** Marks this server-backed resource complete. */
                 old=>old.map(
                /** Transforms each item into the required display or state value. */
                 (value,i)=>i===index?!value:value))} style={s.outcome}><Text style={[s.meta,{flex:1,marginTop:0}]}>{item.resource.description||item.resource.resourceName}</Text><Text style={s.check}>{model.checked[index]?'✓':'○'}</Text></Pressable>)}</View>
            <View style={{flex:1,minHeight:35*scale}}/>
            {!!model.learningError&&<Text accessibilityRole="alert" style={s.meta}>{model.learningError}</Text>}
            <Pressable accessibilityRole="button" disabled={!model.topic || !resources.length || completing} accessibilityLabel="Mark topic complete" onPress={
                /** Opens the complete screen. */
                 async()=>{setCompleting(true);const saved=await model.completeTopic(model.topic);setCompleting(false);if(saved)model.go('complete');}} style={[s.continue,completing&&s.disabled]}><Text style={s.continueText}>{completing?'Saving Progress…':'Mark Topic Complete'}</Text><OpportunityIcon name="arrow" size={24*scale} color="#000"/></Pressable>
        </ScrollView>
        <Modal visible={selected!==null} transparent animationType="fade" onRequestClose={
            /** Closes the selected dialog or detail view. */
             ()=>setSelected(null)}><View style={s.modalBackdrop}><View accessibilityViewIsModal style={s.dialog}><Text accessibilityRole="header" style={s.dialogTitle}>{selected!==null?resources[selected]?.resource.resourceName:''}</Text><Text style={s.meta}>{selected!==null?resources[selected]?.resource.description:''}</Text>{selected!==null&&<Text style={s.resourceDetails}>{resources[selected]?.resource.provider} · {resources[selected]?.resource.estDuration} · {resources[selected]?.resource.difficultyLevel}</Text>}{!!resourceError&&<Text accessibilityRole="alert" style={s.error}>{resourceError}</Text>}{selected!==null&&<Pressable accessibilityRole="link" accessibilityLabel="Open learning resource" onPress={
            /** Opens the selected external learning resource. */
             ()=>void openResource(resources[selected].resource.url)} style={[s.continue,{marginTop:20}]}><Text style={s.continueText}>Open Resource</Text><OpportunityIcon name="arrow" size={24*scale} color="#000"/></Pressable>}<Pressable accessibilityRole="button" accessibilityLabel="Close resource" onPress={
            /** Closes the selected dialog or detail view. */
             ()=>{setSelected(null);setResourceError('');}} style={s.close}><Text style={s.closeText}>Close</Text></Pressable></View></View></Modal>
    </View></View>;
}
const base=StyleSheet.create({
    safe:{flex:1,backgroundColor:'#000'},canvas:{flex:1,width:'100%',alignSelf:'stretch',overflow:'hidden'},content:{flexGrow:1,paddingHorizontal:23},header:{flexDirection:'row',alignItems:'center',marginLeft:-9,opacity:.7,minHeight:44},back:{minWidth:44,minHeight:44,alignItems:'center',justifyContent:'center'},title:{flex:1,fontSize:32,fontWeight:'700',color:'#fff'},intro:{marginTop:100},row:{flexDirection:'row',justifyContent:'space-between',alignItems:'center'},missionLabel:{fontSize:13,lineHeight:16,fontWeight:'700',color:'#d4af53'},iconButton:{minWidth:44,minHeight:44,alignItems:'center',justifyContent:'center',marginRight:-12},description:{fontSize:13,lineHeight:18,color:'#a6a6a4',marginTop:0},badges:{flexDirection:'row',flexWrap:'wrap',gap:8,marginTop:23},badge:{borderRadius:5,paddingHorizontal:8,paddingVertical:2},badgeText:{fontSize:12,lineHeight:16},format:{backgroundColor:'#3c2e1c'},duration:{backgroundColor:'#173646'},complexity:{backgroundColor:'rgba(33,96,80,.7)'},sectionRow:{marginTop:14,marginBottom:10},heading:{fontSize:13,lineHeight:16,fontWeight:'700',color:'#fff'},count:{fontSize:13,lineHeight:16,fontWeight:'700',color:'#d4af53'},empty:{fontSize:13,lineHeight:19,color:'#a6a6a4',padding:16,borderWidth:1,borderColor:'#353e44',borderRadius:10,marginBottom:17},resource:{minHeight:80,flexDirection:'row',alignItems:'center',gap:12,paddingVertical:14,paddingLeft:24,paddingRight:20,marginBottom:17,borderWidth:1,borderColor:'#353e44',borderRadius:10,backgroundColor:'rgba(6,15,20,.85)'},resourceCopy:{flex:1},resourceDetails:{fontSize:12,lineHeight:17,color:'#629fc1',marginTop:5},meta:{fontSize:13,lineHeight:18,color:'#a6a6a4',marginTop:8},error:{fontSize:13,lineHeight:18,color:'#ff9b8f',marginTop:12},learnHeading:{marginBottom:14,marginLeft:3},learnCard:{paddingVertical:8,paddingLeft:24,paddingRight:20,borderWidth:1,borderColor:'#353e44',borderRadius:10,backgroundColor:'rgba(6,15,20,.85)'},outcome:{minHeight:45,flexDirection:'row',alignItems:'center',gap:8,paddingVertical:8},check:{fontSize:16,color:'#d4af53'},continue:{minHeight:54,borderRadius:10,borderWidth:1,borderColor:'#ba7e27',backgroundColor:'#c4872d',flexDirection:'row',alignItems:'center',justifyContent:'center',gap:24,padding:10},disabled:{opacity:.55},continueText:{fontFamily:'PoppinsMedium',fontSize:16,lineHeight:24,color:'#000'},close:{minHeight:44,alignItems:'center',justifyContent:'center',marginTop:12},closeText:{fontSize:14,color:'#a6a6a4'},modalBackdrop:{flex:1,backgroundColor:'rgba(0,0,0,.75)',justifyContent:'center',padding:24},dialog:{width:'100%',maxWidth:430,alignSelf:'center',backgroundColor:'#060f14',borderWidth:1,borderColor:'#353e44',borderRadius:12,padding:20},dialogTitle:{fontSize:20,fontWeight:'700',color:'#fff'},
});
