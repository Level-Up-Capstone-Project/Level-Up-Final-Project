/** Arranges the shared outcome heading, confirmation symbol, message and action. */
import {useEffect} from 'react';
import {Platform,BackHandler,Pressable,ScrollView,StyleSheet,View,useWindowDimensions} from 'react-native';
import {useSafeAreaInsets} from 'react-native-safe-area-context';
import Text from '../components/Text';
import ScreenHeader,{screenHeaderTop} from '../components/ScreenHeader';
import CheckIcon from '../components/CheckIcon';
import LocalVector from '../onboarding/LocalVector';
import type {FeedbackProps} from './types';
/** Arranges the shared outcome heading, confirmation symbol, message and action. */
export default function FeedbackLayout({title,description,button='Continue',onContinue,message}:{title:string;description:string;button?:string}&FeedbackProps){
 const {width,height}=useWindowDimensions(),insets=useSafeAreaInsets(),scale=Math.min(width,480)/408.393;
 useEffect(
     /** Registers the hardware back action and its cleanup. */
      ()=>{
 /** Handles the device back action for this screen. */
 if (Platform.OS === 'web') return;
 const subscription=BackHandler.addEventListener('hardwareBackPress', ()=>{onContinue();return true;});
 /** Removes the registered listeners when the effect ends. */
 return ()=>subscription.remove();},[onContinue]);
 return <View style={s.safe}><View style={s.canvas}>
 <View pointerEvents="none" style={[StyleSheet.absoluteFill,{overflow:'hidden'}]}><View style={{position:'absolute',top:0,opacity:.21}}><LocalVector source={{uri:'/assets/settings/upper-contours.svg'}} width={414.8*scale} height={186.551*scale}/></View><View style={{position:'absolute',bottom:0,opacity:.21}}><LocalVector source={{uri:'/assets/settings/lower-contours.svg'}} width={414.8*scale} height={310.065*scale}/></View></View>
 <ScrollView testID="feedback-screen" contentContainerStyle={[s.content,{minHeight:height,paddingTop:screenHeaderTop(scale,insets.top),paddingBottom:Math.max(24,insets.bottom+16)}]}>
 <ScreenHeader title={title} scale={scale}/><View style={s.middle}><View style={s.emblem}><CheckIcon size={58} color="#d4af53"/></View><Text accessibilityLiveRegion="polite" style={s.copy}>{message||description}</Text></View>
 <Pressable accessibilityRole="button" onPress={onContinue} style={s.button}><Text style={s.buttonText}>{button}</Text></Pressable>
 </ScrollView></View></View>;
}
const s=StyleSheet.create({safe:{flex:1,backgroundColor:'#000'},canvas:{flex:1,width:'100%',maxWidth:480,alignSelf:'center'},content:{flexGrow:1,paddingHorizontal:23},middle:{flex:1,justifyContent:'center',alignItems:'center',paddingVertical:48,gap:28},emblem:{width:124,height:124,borderRadius:62,borderWidth:2,borderColor:'#d4af53',backgroundColor:'#252112',alignItems:'center',justifyContent:'center'},copy:{fontSize:16,lineHeight:25,color:'#f4f1ea',textAlign:'center',maxWidth:350},button:{minHeight:54,padding:14,borderRadius:10,backgroundColor:'#c4872d',borderWidth:1,borderColor:'#ba7e27',alignItems:'center',justifyContent:'center'},buttonText:{fontFamily:'PoppinsMedium',fontSize:16,color:'#000'}});
