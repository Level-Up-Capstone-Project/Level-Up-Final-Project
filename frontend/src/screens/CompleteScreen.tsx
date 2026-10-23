import type {BundledAsset} from '../platform/assets';
/** Displays mission completion details, rewards and related actions. */
import VignettePair from '../components/VignettePair';
import ScreenHeader, {screenHeaderTop} from '../components/ScreenHeader';
import { useState } from 'react';
import { Image, Linking, Modal, Pressable, ScrollView, StyleSheet, View } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import Svg, { Defs, LinearGradient, Rect, Stop } from 'react-native-svg';
import Text from '../components/Text';
import ChevronLeft from '../components/ChevronLeft';
import LocalVector from '../onboarding/LocalVector';
import OpportunityIcon from '../opportunities/OpportunityIcon';
import BottomNavigation from '../navigation/BottomNavigation';
import useResponsiveStyles from '../layout/useResponsiveStyles';
import type { AppModel } from '../navigation/useAppModel';
import KnowledgeCheckAction from '../learning/KnowledgeCheckAction';

/** Displays mission completion details, rewards and related actions. */
export default function CompleteScreen({ model }: { model: AppModel }) {
    const s = useResponsiveStyles(base);
    const scale = Math.min(model.width,480) / 408.393;
    const insets = useSafeAreaInsets();
    const [notice,setNotice] = useState('');
    /** Displays bundled vector artwork at the calculated screen scale. */
    const vector =  (source:BundledAsset,height:number) => <View style={{width:408.8,height,transformOrigin:'top left',transform:[{scale}]}}><LocalVector source={source} width={408.8} height={height}/></View>;
    /** Opens the selected opportunity application link, or displays an availability message. */
    const apply =  async () => {
        const url = model.opportunityModel.selected?.applicationUrl;
        if (!url || !/^https?:\/\//i.test(url)) { setNotice('No company application link is available for this mission yet.'); return; }
        try { await Linking.openURL(url); } catch { setNotice('Unable to open the company application link. Please try again.'); }
    };
    return <View testID="mission-complete-screen" style={s.safe}><View style={s.canvas}>
        <View pointerEvents="none" style={StyleSheet.absoluteFill}>
            <Image testID="mission-landscape" source={{uri:'/assets/mission-complete/landscape.png'}} style={s.landscape} resizeMode="cover"/>
            <VignettePair height={885.372*scale}/>
            <View style={{position:'absolute',top:-14*scale,opacity:.21}}>{vector({uri:'/assets/mission-complete/upper.svg'},186.551)}</View>
            <View style={{position:'absolute',bottom:0,opacity:.21}}>{vector({uri:'/assets/mission-complete/lower.svg'},310.065)}</View>
        </View>
        <ScrollView contentContainerStyle={[s.content,{paddingTop:screenHeaderTop(scale,insets.top),paddingBottom:110*scale+insets.bottom}]}>
            <ScreenHeader title="MISSION COMPLETE" onBack={
                /** Opens the learning screen. */
                 ()=>model.goBack('topic')} backLabel="Back to mission intel" scale={scale} style={{marginBottom:78*scale}}/>
            <Image testID="mission-badge" source={{uri:'/assets/mission-complete/goal-setter.png'}} style={s.badge} resizeMode="contain" accessibilityLabel="Goal Setter mission badge"/>
            <Text style={s.congratulations}>NICE <Text style={s.goldTitle}>WORK!</Text></Text>
            <Text style={s.completed}>You completed “{model.topic}”</Text>
            <View style={s.rewards} accessibilityLabel="Mission rewards pending assessment"><Text style={s.rewardHeading}>MISSION REWARDS</Text><View style={s.rewardRow}>{[['XP EARNED','—'],['READINESS','—'],['SKILL BOOST','—']].map(
                /** Renders a item entry from its data. */
                 ([label,value]) => <View key={label} style={s.rewardColumn}><Text accessibilityLabel={`${label}: not yet assessed`} style={s.rewardValue}>{value}</Text><Text style={s.rewardLabel}>{label}</Text></View>)}</View></View>
            <KnowledgeCheckAction token={model.account.token} item={model.currentTopic}
                displayName={model.account.name || [model.account.firstName,model.account.lastName].filter(Boolean).join(' ')}
                onDashboard={()=>model.go('home')}/>
            <Pressable accessibilityRole="link" accessibilityLabel="Apply on Company Site" onPress={apply} style={s.apply}><Text style={s.applyText}>Apply on Company Site</Text><OpportunityIcon name="arrow" size={24*scale} color="#000"/></Pressable>
        </ScrollView>
        <BottomNavigation screen={model.screen} go={model.go}/>
        <Modal visible={!!notice} transparent animationType="fade" onRequestClose={
            /** Displays feedback for the selected action. */
             () => setNotice('')}><View style={s.scrim}><View style={s.dialog}><Text accessibilityRole="alert" style={s.dialogCopy}>{notice}</Text><Pressable accessibilityRole="button" accessibilityLabel="Close mission message" style={s.close} onPress={
            /** Displays feedback for the selected action. */
             () => setNotice('')}><Text style={s.applyText}>Close</Text></Pressable></View></View></Modal>
    </View></View>;
}
const base = StyleSheet.create({
    safe:{flex:1,backgroundColor:'#000'},canvas:{flex:1,width:'100%',maxWidth:480,alignSelf:'center',overflow:'hidden'},landscape:{position:'absolute',left:-31,top:-4,width:467,height:263},content:{paddingHorizontal:22,flexGrow:1},header:{flexDirection:'row',alignItems:'center',minHeight:44,marginLeft:-10,marginBottom:78},back:{minWidth:44,minHeight:44,justifyContent:'center',alignItems:'center'},title:{flex:1,fontSize:30,fontWeight:'700',color:'#fff'},badge:{width:137,height:137,alignSelf:'center',marginBottom:21},congratulations:{fontSize:24,fontWeight:'700',color:'#fff',textAlign:'center'},goldTitle:{color:'#d4af53',fontSize:24,fontWeight:'700'},completed:{color:'#a6a6a4',fontSize:13,lineHeight:18,textAlign:'center',marginTop:4,marginBottom:12},rewards:{minHeight:130,padding:14,borderRadius:10,borderWidth:1,borderColor:'#353e44',backgroundColor:'rgba(6,15,20,.85)'},rewardHeading:{fontSize:13,fontWeight:'700',color:'#d4af53',textAlign:'center'},rewardRow:{flexDirection:'row',marginTop:25,gap:6},rewardColumn:{flex:1,alignItems:'center'},rewardValue:{fontSize:20,fontWeight:'700',color:'#d4af53'},rewardLabel:{fontSize:13,color:'#a6a6a4',marginTop:12},knowledge:{minHeight:70,marginTop:44,flexDirection:'row',alignItems:'center',padding:12,gap:16,borderRadius:10,borderWidth:1,borderColor:'#353e44',backgroundColor:'rgba(6,15,20,.85)'},flag:{width:43,height:43},knowledgeCopy:{flex:1},knowledgeTitle:{fontSize:13,fontWeight:'700',color:'#fff'},knowledgeMeta:{fontSize:13,lineHeight:18,color:'#a6a6a4',marginTop:4},apply:{minHeight:54,marginTop:52,padding:10,borderRadius:10,borderWidth:1,borderColor:'#ba7e27',backgroundColor:'#c4872d',flexDirection:'row',alignItems:'center',justifyContent:'center',gap:12},applyText:{fontSize:16,fontFamily:'PoppinsMedium',color:'#000'},scrim:{flex:1,padding:24,backgroundColor:'rgba(0,0,0,.7)',alignItems:'center',justifyContent:'center'},dialog:{width:'100%',maxWidth:420,padding:20,borderRadius:16,borderWidth:1,borderColor:'#353e44',backgroundColor:'#0b151c'},dialogCopy:{fontSize:14,lineHeight:22,color:'#fff',marginBottom:20},close:{minHeight:44,borderRadius:8,backgroundColor:'#d4af53',alignItems:'center',justifyContent:'center'},
});
