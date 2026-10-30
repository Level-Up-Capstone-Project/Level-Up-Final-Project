import type {BundledAsset} from '../platform/assets';
/** Displays the learning route and progress through its missions. */
import VignettePair from '../components/VignettePair';
import ScreenHeader, {screenHeaderTop} from '../components/ScreenHeader';
import { useEffect } from 'react';
import { Platform, BackHandler, Image, Pressable, ScrollView, StyleSheet, View } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import Text from '../components/Text';
import OpportunityIcon from '../opportunities/OpportunityIcon';
import LocalVector from '../onboarding/LocalVector';
import useResponsiveStyles from '../layout/useResponsiveStyles';
import type { AppModel } from '../navigation/useAppModel';

/** Displays the learning route and progress through its missions. */
export default function MissionRoadmapScreen({model}:{model:AppModel}) {
    const roles=model.roles;
    const {learningGroups,resourcesForTopic}=model;
    const s=useResponsiveStyles(base),insets=useSafeAreaInsets();
    const scale=Math.min(model.width,480)/408.393;
    /** Collects the topic names from the learning groups. */
    const topics=learningGroups.flatMap( group=>group.topics);
    /** Counts the completed resource steps for a topic. */
    const done= (topic:string)=>resourcesForTopic(topic).filter(
        /** Selects the resources that match the current conditions. */
         (_,i)=>model.completed.includes(topic)||!!model.taskProgress[topic]?.[i]).length;
    /** Adds a topic resource count to the total number of learning steps. */
    const totalSteps=topics.reduce( (sum,topic)=>sum+resourcesForTopic(topic).length,0);
    /** Adds the completed steps for a topic to the overall count. */
    const completedSteps=topics.reduce( (sum,topic)=>sum+done(topic),0);
    const completedLessons=topics.filter(
        /** Selects the topics that match the current conditions. */
         topic=>done(topic)===resourcesForTopic(topic).length).length;
    const percent=totalSteps?Math.round(completedSteps/totalSteps*100):0;
    /** Checks whether a mission contains unfinished topics. */
    const activeMission=learningGroups.findIndex( group=>group.topics.some(
        /** Checks the topic against the current selection or criteria. */
         topic=>done(topic)<resourcesForTopic(topic).length));
    /** Selects the next unfinished topic from the available topics. */
    const nextTopic= (options:string[])=>{
        if(options.includes(model.topic)&&done(model.topic)>0&&done(model.topic)<resourcesForTopic(model.topic).length)return model.topic;
        return options.find(
            /** Checks the item against the current selection or criteria. */
             topic=>done(topic)<resourcesForTopic(topic).length)||options[0];
    };
    /** Selects a learning topic and opens its mission details. */
    const open= (topic:string)=>{if(!topic)return;model.setTopic(topic);model.go('topic');};
    const continueLabel=topics.length
        ? percent===100?'Review Learning':'Continue Learning'
        : model.learningLoading?'Loading Roadmap…'
        : model.learningError?'Retry Roadmap'
        :'Choose a Career';
    /** Continues the roadmap or directs the user to resolve its empty/error state. */
    const continueLearning=()=>{
        if(topics.length){open(nextTopic(topics));return;}
        if(model.learningLoading)return;
        if(model.learningError){void model.reloadLearning();return;}
        model.go('profileSettings');
    };
    useEffect(
        /** Registers the hardware back action and its cleanup. */
         ()=>{
    /** Handles the device back action for this screen. */
    if (Platform.OS === 'web') return;
    const back=BackHandler.addEventListener('hardwareBackPress', ()=>{model.goBack('missionHub');return true;});
    /** Removes the registered listeners when the effect ends. */
    return  ()=>back.remove();},[model.goBack]);
    /** Displays bundled vector artwork at the calculated screen scale. */
    const vector= (source:BundledAsset,height:number)=><View style={{width:408.8,height,transformOrigin:'top left',transform:[{scale}]}}><LocalVector source={source} width={408.8} height={height}/></View>;
    return <View style={s.safe}><View testID="mission-roadmap-screen" style={s.canvas}>
        <View pointerEvents="none" style={StyleSheet.absoluteFill}>
            <Image testID="roadmap-landscape" source={{uri:'/assets/roadmap/landscape.png'}} resizeMode="stretch" style={{position:'absolute',left:0,top:.6*scale,width:412.35*scale,height:232.06*scale}}/>
            <VignettePair height={885.372*scale}/>
            <View style={{position:'absolute',top:.22*scale,left:.22*scale,opacity:.35}}>{vector({uri:'/assets/home/upper.svg'},186.551)}</View>
            <View style={{position:'absolute',bottom:0,left:.22*scale,opacity:.35}}>{vector({uri:'/assets/home/lower.svg'},310.065)}</View>
        </View>
        <ScrollView testID="roadmap-content" contentContainerStyle={[s.content,{paddingTop:screenHeaderTop(scale,insets.top),paddingBottom:Math.max(25*scale,insets.bottom+8)}]}>
            <ScreenHeader title="MISSION ROADMAP" onBack={
                /** Returns to the previous screen, falling back to the mission hub. */
                 ()=>model.goBack('missionHub')} backLabel="Back to previous page" scale={scale}/>
            <View style={s.personal}><Text style={s.eyebrow}>YOUR PERSONAL MISSION ROUTE</Text><Text style={s.target}>Target: {model.roadmapCareerName || roles[model.role] || 'Set your target roles'}</Text></View>
            <View style={s.summary}><View style={s.row}><Text style={s.heading}>Mission Progress</Text><Text testID="roadmap-percent" style={s.percent}>{percent}%</Text></View><Text testID="roadmap-lessons" style={s.meta}>{completedLessons} of {topics.length} lessons completed</Text><View accessibilityRole="progressbar" accessibilityLabel="Overall mission progress" accessibilityValue={{min:0,max:100,now:percent}} style={[s.track,{marginTop:10*scale}]}><View style={[s.fill,{width:`${percent}%`}]}/></View></View>
            <Text style={s.section}>LEARNING ROADMAP</Text>{!topics.length && <Text style={s.meta}>{model.learningLoading?'Loading your learning roadmap…':model.learningError||'Your learning roadmap will appear when recommendations are available.'}</Text>}
            {!topics.length && !!model.careerSuggestions.length && <View style={s.suggestions}>
                <Text style={s.suggestionHeading}>CAREER SUGGESTIONS</Text>
                <Text style={s.meta}>Choose a suggested career to build a personalized learning roadmap.</Text>
                {model.careerSuggestions.map(career=><View key={career.careerID} style={s.suggestion}>
                    <Text style={s.heading}>{career.careerName}</Text>
                    {!!career.matchedSkills.length && <Text style={s.meta}>Matched skills: {career.matchedSkills.map(skill=>skill.skillName).join(', ')}</Text>}
                    <Pressable accessibilityRole="button" accessibilityState={{disabled:model.learningLoading}}
                        disabled={model.learningLoading} onPress={()=>void model.generateRoadmap(career.careerID)}
                        style={[s.generate,model.learningLoading&&s.disabled]}>
                        <Text style={s.generateText}>{model.generatingCareerID===career.careerID?'Building roadmap…':'Build this roadmap'}</Text>
                    </Pressable>
                </View>)}
            </View>}
            {!topics.length && !model.learningLoading && !model.careerSuggestions.length && <View style={s.emptySuggestions}>
                <Text style={s.meta}>Analyse your CV or transcript for career suggestions, or choose a target career in your profile.</Text>
                <Pressable accessibilityRole="button" onPress={()=>model.go('profileSettings')} style={s.generate}>
                    <Text style={s.generateText}>Choose a target career</Text>
                </Pressable>
            </View>}
            {learningGroups.map(
                /** Renders a group entry from its data. */
                 (group,index)=>{
                /** Adds resource counts to the mission step total. */
                const total=group.topics.reduce( (sum,topic)=>sum+resourcesForTopic(topic).length,0);
                /** Adds completed resource counts to the mission progress. */
                const completed=group.topics.reduce( (sum,topic)=>sum+done(topic),0);
                const status=completed===total?'COMPLETED':completed>0||index===activeMission?'IN PROGRESS':'UP NEXT';
                const statusStyle=status==='COMPLETED'?s.statusCompleted:status==='IN PROGRESS'?s.statusInProgress:s.statusUpNext;
                const statusTextStyle=status==='COMPLETED'?s.textCompleted:status==='IN PROGRESS'?s.textInProgress:s.textUpNext;
                return <Pressable key={group.title} accessibilityRole="button" accessibilityLabel={`Mission ${index+1}: ${group.title}, ${completed} of ${total} steps completed`} onPress={
                    /** Handles the on press for this component. */
                     ()=>open(nextTopic(group.topics))} style={s.mission}>
                    <Image testID={'roadmap-mission-icon-'+(index+1)} source={{uri:'/assets/mission-complete/knowledge-check.png'}} resizeMode="contain" style={s.flag}/>
                    <View style={s.missionCopy}><View style={s.row}><Text style={s.heading}>Mission {index+1}</Text><View testID={'mission-status-'+(index+1)} style={[s.status,statusStyle]}><Text style={[s.statusText,statusTextStyle]}>{status}</Text></View></View><Text style={s.meta}>{group.title}</Text><View accessibilityRole="progressbar" accessibilityLabel={'Mission '+(index+1)+' progress'} accessibilityValue={{min:0,max:total,now:completed}} style={[s.track,{marginTop:6*scale}]}><View style={[s.fill,{width:`${total?completed/total*100:0}%`}]}/></View><Text testID={'mission-step-count-'+(index+1)} style={s.steps}>{completed} / {total} STEPS COMPLETED</Text></View>
                </Pressable>;
            })}
            <View style={{flex:1,minHeight:35*scale}}/>
            <Pressable testID="roadmap-continue" accessibilityRole="button" disabled={model.learningLoading}
                accessibilityState={{disabled:model.learningLoading}} accessibilityLabel={continueLabel}
                onPress={continueLearning} style={[s.continue,model.learningLoading&&s.disabled]}>
                <Text style={s.continueText}>{continueLabel}</Text><OpportunityIcon name="arrow" size={24*scale} color="#000"/>
            </Pressable>
        </ScrollView>
    </View></View>;
}
const base=StyleSheet.create({
    safe:{flex:1,backgroundColor:'#000'},canvas:{flex:1,width:'100%',alignSelf:'stretch',overflow:'hidden'},content:{flexGrow:1,paddingHorizontal:23},header:{flexDirection:'row',alignItems:'center',marginLeft:-9,opacity:.7,minHeight:44},back:{minWidth:44,minHeight:44,alignItems:'center',justifyContent:'center'},title:{flex:1,fontSize:30,fontWeight:'700',color:'#fff'},personal:{marginTop:38,paddingHorizontal:6},eyebrow:{color:'#d4af53',fontSize:13,lineHeight:16,fontWeight:'700'},target:{fontSize:13,lineHeight:16,fontWeight:'700',color:'#fff',marginTop:2},
    summary:{marginTop:45,minHeight:95,padding:16,borderWidth:1,borderColor:'#353e44',borderRadius:10,backgroundColor:'rgba(6,15,20,.85)'},row:{flexDirection:'row',justifyContent:'space-between',alignItems:'center'},heading:{fontSize:13,lineHeight:16,fontWeight:'700',color:'#fff'},percent:{fontSize:20,lineHeight:24,fontWeight:'700',color:'#d4af53'},meta:{fontSize:13,lineHeight:16,color:'#a6a6a4',marginTop:3},track:{height:6,borderRadius:50,backgroundColor:'#a6a6a4',overflow:'hidden'},fill:{height:'100%',backgroundColor:'#d4af53',borderRadius:50},section:{fontSize:13,lineHeight:16,fontWeight:'700',color:'#fff',marginTop:20,marginBottom:10},suggestions:{gap:10,marginBottom:16},suggestionHeading:{fontSize:13,lineHeight:16,fontWeight:'700',color:'#d4af53',marginTop:12},suggestion:{padding:14,borderWidth:1,borderColor:'#353e44',borderRadius:10,backgroundColor:'rgba(6,15,20,.85)'},generate:{minHeight:42,alignItems:'center',justifyContent:'center',paddingHorizontal:14,marginTop:10,borderRadius:8,backgroundColor:'#c4872d'},generateText:{fontSize:14,fontWeight:'700',color:'#000'},disabled:{opacity:.6},emptySuggestions:{marginBottom:16},mission:{minHeight:80,flexDirection:'row',alignItems:'center',gap:14,paddingHorizontal:13,paddingVertical:8,marginBottom:17,borderWidth:1,borderColor:'#353e44',borderRadius:10,backgroundColor:'rgba(6,15,20,.85)'},status:{borderRadius:5,paddingHorizontal:10,paddingVertical:2},statusText:{fontSize:12,lineHeight:16},statusCompleted:{backgroundColor:'rgba(33,96,80,.7)'},statusInProgress:{backgroundColor:'#3c2e1c'},statusUpNext:{backgroundColor:'#173646'},textCompleted:{color:'#6de0b3'},textInProgress:{color:'#c6a572'},textUpNext:{color:'#629fc1'},flag:{width:43,height:43},missionCopy:{flex:1},steps:{fontSize:12,lineHeight:16,color:'#a6a6a4',marginTop:3},continue:{minHeight:54,borderRadius:10,borderWidth:1,borderColor:'#ba7e27',backgroundColor:'#c4872d',flexDirection:'row',alignItems:'center',justifyContent:'center',gap:24,padding:10},continueText:{fontFamily:'PoppinsMedium',fontSize:16,lineHeight:24,color:'#000'},
});
