/** Displays the loading transition before document onboarding. */
import { useEffect, useRef, useState } from 'react';
import { StyleSheet, View } from 'react-native';
import LocalVector from '../onboarding/LocalVector';
import Text from '../components/Text';
import RadarLoader from '../loader/RadarLoader';

/** Displays the loading transition before document onboarding. */
export default function LoaderScreen({ onComplete }: { onComplete: () => void }) {
    const [canvas,setCanvas]=useState({width:408.393,height:885.372});
    const complete=useRef(onComplete);
    useEffect(
        /** Synchronises component state with the effect dependencies. */
         ()=>{complete.current=onComplete;},[onComplete]);
    useEffect(
        /** Synchronises component state with the effect dependencies. */
         ()=>{
    /** Opens the next screen after the loading transition. */
    const timer=setTimeout( ()=>complete.current(),2200);
    /** Clears the pending timer when the effect ends. */
    return  ()=>clearTimeout(timer);},[]);
    const scale=canvas.width/408.393;
    return <View style={styles.safe}>
        <View testID="loader-screen" style={styles.canvas} onLayout={
            /** Records the measured layout dimensions for this view. */
             ({nativeEvent:{layout}})=>setCanvas({width:layout.width,height:layout.height})}>
            <View testID="loader-upper-contours" pointerEvents="none" style={{position:'absolute',left:-26.78*scale,top:-36.18*scale,transformOrigin:'top left',transform:[{scale}]}}><LocalVector source={{uri:'/assets/loader/upper-contours.svg'}} width={462.8} height={198.328}/></View>
            <View testID="loader-lower-contours" pointerEvents="none" style={{position:'absolute',left:-26.78*scale,top:canvas.height-294.63*scale,transformOrigin:'top left',transform:[{scale}]}}><LocalVector source={{uri:'/assets/loader/lower-contours.svg'}} width={462.8} height={329.673}/></View>
            <View accessible accessibilityRole="progressbar" accessibilityLabel="Deploying troops. Loading" accessibilityState={{busy:true}} style={{position:'absolute',left:(canvas.width-150*scale)/2,top:canvas.height/2-214.686*scale}}><RadarLoader size={150*scale}/></View>
            <Text accessibilityRole="header" style={{position:'absolute',top:canvas.height/2-15.686*scale,width:'100%',textAlign:'center',fontFamily:'Inter',fontSize:26*scale,fontWeight:'700',color:'#fff'}}>DEPLOYING TROOPS...</Text>
        </View>
    </View>;
}
const styles=StyleSheet.create({safe:{flex:1,backgroundColor:'#000'},canvas:{flex:1,width:'100%',maxWidth:480,alignSelf:'center',overflow:'hidden',backgroundColor:'#000'}});
