/** Displays the animated radar loading indicator. */
import { useEffect, useRef, useState } from 'react';
import { AccessibilityInfo, Animated, AppState, Easing, StyleSheet, View } from 'react-native';
import Svg, { Circle, Defs, Line, Path, RadialGradient, Stop } from 'react-native-svg';

// React Native adaptation of mrhyddenn's Uiverse radar; Figma gold variant.
/** Displays the animated radar loading indicator. */
export default function RadarLoader({ size }: { size:number }) {
    const rotation=useRef(new Animated.Value(0)).current;
    const [reduced,setReduced]=useState(false);
    const [active,setActive]=useState(AppState.currentState === 'active' || AppState.currentState == null);
    useEffect(
        /** Synchronises component state with the effect dependencies. */
         ()=>{
        let mounted=true;
        AccessibilityInfo.isReduceMotionEnabled().then(
            /** Stores the successful result of the asynchronous operation. */
             value=>{if(mounted)setReduced(value);});
        const motion=AccessibilityInfo.addEventListener('reduceMotionChanged',setReduced);
        /** Handles the registered device or application event. */
        const state=AppState.addEventListener('change', value=>setActive(value==='active'));
        /** Removes the registered listeners when the effect ends. */
        return  ()=>{mounted=false;motion.remove();state.remove();};
    },[]);
    useEffect(
        /** Updates the component animation and its cleanup. */
         ()=>{
        rotation.setValue(0);
        if(reduced || !active)return;
        const animation=Animated.loop(Animated.timing(rotation,{toValue:1,duration:2000,easing:Easing.linear,useNativeDriver:false,isInteraction:false}));
        animation.start();
        /** Runs the effect cleanup when its dependencies change or the component closes. */
        return  ()=>{animation.stop();};
    },[rotation,reduced,active]);
    /** Draws one segment of the radar sweep with its calculated opacity. */
    const sweep=Array.from({length:55}, (_,index)=>{
        const a=(-100+index*2)*Math.PI/180,b=a+2.2*Math.PI/180;
        const alpha=Math.exp(-Math.pow((index*2-65)/30,2));
        return <Path key={index} d={`M75 75L${75+75*Math.cos(a)} ${75+75*Math.sin(a)}A75 75 0 0 1 ${75+75*Math.cos(b)} ${75+75*Math.sin(b)}Z`} fill="url(#radar-gold)" opacity={alpha}/>;
    });
    return <View testID="radar-loader" pointerEvents="none" style={{width:size,height:size,borderRadius:size/2,borderWidth:1,borderColor:'#333',overflow:'hidden',backgroundColor:'transparent',boxShadow:'25px 25px 75px rgba(0,0,0,.55)'}}>
        <Animated.View testID="radar-sweep" style={[StyleSheet.absoluteFill,{transform:[{rotate:rotation.interpolate({inputRange:[0,1],outputRange:['146.99deg','506.99deg']})}]}]}>
            <Svg width="100%" height="100%" viewBox="0 0 150 150">
                <Defs><RadialGradient id="radar-gold" gradientUnits="userSpaceOnUse" cx={75} cy={75} r={75}><Stop offset={0} stopColor="#d4af53" stopOpacity={0}/><Stop offset={.3} stopColor="#d4af53" stopOpacity={.15}/><Stop offset={.7} stopColor="#d4af53" stopOpacity={.95}/><Stop offset={1} stopColor="#d4af53" stopOpacity={.9}/></RadialGradient></Defs>
                {sweep}
                <Line x1={75} y1={75} x2={150} y2={75} stroke="#fff" strokeWidth={.7} strokeDasharray="2 2" opacity={.75}/>
            </Svg>
        </Animated.View>
        <Svg width="100%" height="100%" viewBox="0 0 150 150" style={StyleSheet.absoluteFill}><Circle cx={75} cy={75} r={54} fill="none" stroke="#444" strokeWidth={1} strokeDasharray="3 2"/><Circle cx={75} cy={75} r={26} fill="none" stroke="#444" strokeWidth={1} strokeDasharray="3 2"/></Svg>
    </View>;
}
