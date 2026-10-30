import type {BundledAsset} from '../platform/assets';
/** Displays the campaign medal catalogue, filters and medal details. */
import ScreenHeader, {screenHeaderTop} from '../components/ScreenHeader';
import { useEffect, useState } from 'react';
import { Platform, BackHandler, Image, Modal, Pressable, ScrollView, StyleSheet, View } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import Text from '../components/Text';
import LocalVector from '../onboarding/LocalVector';
import { badges,categories,type Badge } from '../achievements/data';
import type { AppModel } from '../navigation/useAppModel';

/** Displays the campaign medal catalogue, filters and medal details. */
export default function AchievementsScreen({model}:{model:AppModel}) {
    const scale=Math.min(model.width,480)/408.393;
    const insets=useSafeAreaInsets();
    const [category,setCategory]=useState<typeof categories[number]>('All');
    const [selected,setSelected]=useState<Badge|null>(null);
    const earned=new Set(model.earnedBadgeIds);
    const count=badges.filter(
        /** Selects the badges that match the current conditions. */
         badge=>earned.has(badge.id)).length;
    /** Checks whether the medal belongs to the selected category. */
    const visible=badges.filter( badge=>category==='All'||badge.category===category);
    useEffect(
        /** Registers the hardware back action and its cleanup. */
         ()=>{
    /** Handles the device back action for this screen. */
    if (Platform.OS === 'web') return;
    const back=BackHandler.addEventListener('hardwareBackPress', ()=>{model.goBack('home');return true;});
    /** Removes the registered listeners when the effect ends. */
    return  ()=>back.remove();},[model.goBack]);
    /** Displays bundled vector artwork at the calculated screen scale. */
    const vector= (source:BundledAsset,width:number,height:number)=><View style={{width,height,transformOrigin:'top left',transform:[{scale}]}}><LocalVector source={source} width={width} height={height}/></View>;
    return <View style={s.safe}><View testID="achievements-screen" style={s.canvas}>
        <View pointerEvents="none" style={StyleSheet.absoluteFill}>
            <View style={{position:'absolute',top:.22*scale,left:.22*scale,opacity:.35}}>{vector({uri:'/assets/achievements/35e83.svg'},408.8,131.683)}</View>
            {[
                {source:{uri:'/assets/achievements/a3d9e.svg'},top:135.02,height:51.786},
                {source:{uri:'/assets/achievements/0ab92.svg'},top:120.05,height:46.5499},
                {source:{uri:'/assets/achievements/e7a6c.svg'},top:108.19,height:44.6197},
                {source:{uri:'/assets/achievements/705e0.svg'},top:97.92,height:43.7452},
            ].map(
                /** Renders a badge entry from its data. */
                 (line,i)=><View key={i} style={{position:'absolute',top:line.top*scale,left:.2*scale}}>{vector(line.source,408.8,line.height)}</View>)}
        </View>
        <ScrollView contentContainerStyle={{paddingTop:screenHeaderTop(scale,insets.top),paddingBottom:Math.max(8*scale,insets.bottom)}}>
            <ScreenHeader title="CAMPAIGN MEDALS" onBack={
                /** Opens the home screen. */
                 ()=>model.goBack('home')} backLabel="Back to previous page" scale={scale} style={{marginBottom:12*scale}}/>
            <Image testID="operation-protea" source={{uri:'/assets/achievements/5ea1e.png'}} resizeMode="contain" accessibilityLabel="Operation Protea emblem" style={{width:169*scale,height:169*scale,alignSelf:'center'}}/>
            <Text style={[s.operation,{fontSize:16*scale,lineHeight:19*scale,marginTop:4*scale}]}>OPERATION PROTEA</Text>
            <Text style={[s.description,{fontSize:Math.max(11,12*scale),lineHeight:15*scale,marginTop:4*scale,paddingHorizontal:24*scale}]}>Complete your career profile and earn your mission badges.</Text>
            <View style={{flexDirection:'row',alignItems:'center',gap:14*scale,marginTop:14*scale,marginHorizontal:33*scale,height:15*scale}}>
                <View accessibilityRole="progressbar" accessibilityLabel="Mission badges earned" accessibilityValue={{min:0,max:badges.length,now:count}} style={s.track}><View testID="achievement-progress" style={[s.fill,{width:`${count/badges.length*100}%`}]}/></View>
                <Text testID="achievement-count" style={[s.count,{fontSize:12*scale}]}>{count} / {badges.length}</Text>
            </View>
            <View style={[s.badgePanel,{marginTop:26*scale}]}>
                <View accessibilityRole="tablist" style={{flexDirection:'row',height:42*scale,minHeight:44}}>
                    {categories.map(
                        /** Renders a badge entry from its data. */
                         item=><Pressable key={item} accessibilityRole="tab" accessibilityLabel={item} accessibilityState={{selected:category===item}} onPress={
                        /** Handles the on press for this component. */
                         ()=>setCategory(item)} style={s.tab}><Text style={{fontSize:Math.max(11,12*scale),color:category===item?'#d4af53':'#e2e2e2',fontWeight:category===item?'700':'400'}}>{item}</Text>{category===item && <View style={{position:'absolute',bottom:0}}>{vector({uri:'/assets/achievements/aafe2.svg'},34,3)}</View>}</Pressable>)}
                </View>
                <View pointerEvents="none" style={{height:scale,overflow:'hidden'}}>{vector({uri:'/assets/achievements/7ac48.svg'},411,1)}</View>
                <View testID="achievement-grid" style={{flexDirection:'row',flexWrap:'wrap',paddingHorizontal:12*scale,paddingTop:12*scale,paddingBottom:8*scale}}>
                    {visible.map(
                        /** Renders a badge entry from its data. */
                         badge=><Pressable key={badge.id} accessibilityRole="button" disabled={!earned.has(badge.id)} accessibilityState={{disabled: !earned.has(badge.id)}} accessibilityLabel={badge.name+(earned.has(badge.id)?', earned':', locked. '+badge.description)} onPress={
                        /** Selects the item to display in the detail view. */
                         ()=>{if(earned.has(badge.id))setSelected(badge);}} style={{width:'33.3333%',minHeight:148*scale,alignItems:'center',paddingHorizontal:3*scale,paddingBottom:12*scale,opacity: earned.has(badge.id)?1:.35}}>
                        <Image testID={'badge-'+badge.id} source={badge.image} resizeMode="contain" accessible={false} style={{width:88*scale,height:88*scale}}/>
                        <Text style={{color:'#fff',fontWeight:'700',fontSize:Math.max(10,11*scale),textAlign:'center',marginTop:6*scale}}>{badge.name}</Text><Text style={{color:'#e7c580',fontSize:Math.max(9,9*scale),lineHeight:13,textAlign:'center',marginTop:4*scale}}>{badge.description}</Text>
                    </Pressable>)}
                </View>
            </View>
        </ScrollView>
        <Modal visible={!!selected && earned.has(selected.id)} transparent animationType="fade" onRequestClose={
            /** Closes the selected dialog or detail view. */
             ()=>setSelected(null)}><View style={s.scrim}><View style={s.dialog}>{selected && <><Image source={selected.image} style={s.detailBadge} resizeMode="contain" accessible={false}/><Text accessibilityRole="header" style={s.detailTitle}>{selected.name}</Text><Text style={s.detailCopy}>{selected.description}{'\n\n'}{earned.has(selected.id)?'Earned':'Not earned yet'}</Text></>}<Pressable accessibilityRole="button" accessibilityLabel="Close badge details" onPress={
            /** Closes the selected dialog or detail view. */
             ()=>setSelected(null)} style={s.close}><Text style={s.closeText}>Close</Text></Pressable></View></View></Modal>
    </View></View>;
}
const s=StyleSheet.create({
    safe:{flex:1,backgroundColor:'#000'},canvas:{flex:1,width:'100%',alignSelf:'stretch',overflow:'hidden'},
    header:{flexDirection:'row',alignItems:'center',opacity:.7},back:{minWidth:44,minHeight:44,justifyContent:'center',alignItems:'center'},heading:{color:'#fff',fontWeight:'700',flex:1},
    operation:{color:'#d4af53',fontWeight:'700',textAlign:'center'},description:{color:'#fff',textAlign:'center'},
    track:{flex:1,height:6,borderRadius:50,backgroundColor:'#a6a6a4',overflow:'hidden'},fill:{height:'100%',borderRadius:50,backgroundColor:'#d4af53'},count:{color:'#e2e2e2',minWidth:48},
    badgePanel:{backgroundColor:'rgba(6,15,20,.85)',borderTopWidth:1,borderColor:'#353e44'},tab:{flex:1,minHeight:44,alignItems:'center',justifyContent:'center'},
    scrim:{flex:1,backgroundColor:'rgba(0,0,0,.7)',padding:24,alignItems:'center',justifyContent:'center'},dialog:{width:'100%',maxWidth:420,backgroundColor:'#0b151c',padding:20,borderRadius:16,borderWidth:1,borderColor:'#353e44'},detailBadge:{width:150,height:150,alignSelf:'center'},detailTitle:{color:'#d4af53',fontSize:20,fontWeight:'700',textAlign:'center',marginTop:12},detailCopy:{color:'#a6a6a4',fontSize:14,textAlign:'center',marginVertical:20},close:{minHeight:44,backgroundColor:'#d4af53',borderRadius:8,alignItems:'center',justifyContent:'center'},closeText:{fontSize:14,fontWeight:'700',color:'#000'},
});
