/** Defines an animated sun-and-moon theme switch component. */
// Adapted from RiccardoRapelli's Uiverse switch (MIT). See THIRD_PARTY_NOTICES.md.
import { useEffect, useRef, useState } from 'react';
import { AccessibilityInfo, Animated, Pressable, StyleSheet, View } from 'react-native';
import Svg, { Circle, Path } from 'react-native-svg';

const starPath = 'M0 10C10 10 10 10 0 10C10 10 10 10 10 20C10 10 10 10 20 10C10 10 10 10 10 0C10 10 10 10 0 10Z';
/** Defines an animated sun-and-moon theme switch component. */
export default function ThemeSwitch({ value, onValueChange }: { value: boolean; onValueChange: (value: boolean) => void }) {
    const transition = useRef(new Animated.Value(value ? 1 : 0)).current;
    const cloud = useRef(new Animated.Value(0)).current;
    const twinkle = useRef(new Animated.Value(1)).current;
    const [reduceMotion, setReduceMotion] = useState(false);
    const [focused, setFocused] = useState(false);
    useEffect(
        /** Synchronises component state with the effect dependencies. */
         () => {
        AccessibilityInfo.isReduceMotionEnabled().then(setReduceMotion);
        const subscription = AccessibilityInfo.addEventListener('reduceMotionChanged', setReduceMotion);
        /** Removes the registered listeners when the effect ends. */
        return  () => subscription.remove();
    }, []);
    // Opacity and transforms share Animated.View nodes. Keep this entire graph
    // JS-driven so native transforms cannot promote the shared transition value.
    useEffect(
        /** Updates the component animation and its cleanup. */
         () => {
        const animation = Animated.timing(transition, { toValue: value ? 1 : 0, duration: reduceMotion ? 0 : 400, useNativeDriver: false });
        animation.start();
        /** Runs the effect cleanup when its dependencies change or the component closes. */
        return  () => animation.stop();
    }, [value, reduceMotion, transition]);
    useEffect(
        /** Updates the component animation and its cleanup. */
         () => {
        if (reduceMotion) { cloud.setValue(0); twinkle.setValue(1); return; }
        const clouds = Animated.loop(Animated.sequence([Animated.timing(cloud,{toValue:4,duration:2400,useNativeDriver:false}),Animated.timing(cloud,{toValue:-4,duration:2400,useNativeDriver:false}),Animated.timing(cloud,{toValue:0,duration:1200,useNativeDriver:false})]));
        const stars = Animated.loop(Animated.sequence([Animated.timing(twinkle,{toValue:1.2,duration:800,useNativeDriver:false}),Animated.timing(twinkle,{toValue:.8,duration:800,useNativeDriver:false}),Animated.timing(twinkle,{toValue:1,duration:400,useNativeDriver:false})]));
        clouds.start(); stars.start();
        /** Runs the effect cleanup when its dependencies change or the component closes. */
        return  () => { clouds.stop(); stars.stop(); };
    }, [cloud, twinkle, reduceMotion]);
    const inverse = transition.interpolate({ inputRange:[0,1],outputRange:[1,0] });
    return <Pressable accessibilityRole="switch" accessibilityLabel="Dark theme" accessibilityState={{ checked:value }} aria-checked={value} onPress={
        /** Applies the selected toggle or checkbox value. */
         () => onValueChange(!value)} onFocus={
        /** Updates input focus and its visual state. */
         () => setFocused(true)} onBlur={
        /** Updates input focus and its visual state. */
         () => setFocused(false)} style={[styles.target, focused && styles.focus]}>
        <Animated.View pointerEvents="none" style={[styles.track, { backgroundColor:transition.interpolate({inputRange:[0,1],outputRange:['#2196f3','#000']}) }]}>
            <Animated.View style={[styles.stars,{opacity:transition,transform:[{translateY:transition.interpolate({inputRange:[0,1],outputRange:[-32,0]})}]}]}>
                {[[3,2,20],[3,16,6],[10,20,12],[18,0,18]].map(
                    /** Renders a item entry from its data. */
                     ([left,top,size],i) => <Animated.View key={i} style={{position:'absolute',left,top,width:size,height:size,transform:[{scale:twinkle}]}}><Svg width={size} height={size} viewBox="0 0 20 20"><Path d={starPath} fill="white"/></Svg></Animated.View>)}
            </Animated.View>
            <Animated.View style={[styles.orb,{backgroundColor:transition.interpolate({inputRange:[0,1],outputRange:['#ffff00','#fff']}),transform:[{translateX:transition.interpolate({inputRange:[0,1],outputRange:[0,26]})},{rotate:transition.interpolate({inputRange:[0,1],outputRange:['0deg','360deg']})}]}]}>
                <Animated.View style={[StyleSheet.absoluteFill,{opacity:inverse}]}>{[[-8,-8,43],[-13,-13,55],[-18,-18,60]].map(
                    /** Renders a item entry from its data. */
                     ([left,top,size],i)=><View key={i} style={{position:'absolute',left,top,width:size,height:size,borderRadius:size/2,backgroundColor:'rgba(255,255,255,.1)'}}/>)}</Animated.View>
                <Animated.View style={[StyleSheet.absoluteFill,{opacity:transition}]}><Svg width={26} height={26}><Circle cx={13} cy={6} r={3} fill="gray"/><Circle cx={7} cy={15} r={5} fill="gray"/><Circle cx={17.5} cy={19.5} r={1.5} fill="gray"/></Svg></Animated.View>
            </Animated.View>
            <Animated.View style={[StyleSheet.absoluteFill,{opacity:inverse,transform:[{translateX:cloud}]}]}>{[[30,15,40,'#ccc'],[44,10,20,'#ccc'],[18,24,30,'#ccc'],[36,18,40,'#eee'],[48,14,20,'#eee'],[22,26,30,'#eee']].map(
                /** Renders a item entry from its data. */
                 ([left,top,size,color],i)=><View key={i} style={{position:'absolute',left:left as number,top:top as number,width:size as number,height:size as number,borderRadius:(size as number)/2,backgroundColor:color as string}}/>)}</Animated.View>
            <View style={styles.goldOutline}/>
        </Animated.View>
    </Pressable>;
}
const styles = StyleSheet.create({target:{minWidth:64,minHeight:44,alignItems:'center',justifyContent:'center',borderRadius:22},focus:{outlineColor:'#d4af53',outlineWidth:2,outlineStyle:'solid'},goldOutline:{position:'absolute',top:0,right:0,bottom:0,left:0,borderWidth:2,borderColor:'#d4af53',borderRadius:34},track:{width:60,height:34,borderRadius:34,overflow:'hidden'},orb:{position:'absolute',left:4,bottom:4,width:26,height:26,borderRadius:13},stars:{position:'absolute',top:0,left:0,width:36,height:34}});
