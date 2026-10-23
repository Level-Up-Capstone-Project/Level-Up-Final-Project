/** Displays a centred screen title with an optional back button. */
import {useState} from 'react';
import {Pressable,StyleSheet,View,type StyleProp,type ViewStyle} from 'react-native';
import Text from './Text';
import ChevronLeft from './ChevronLeft';
/** Calculates the screen heading position from its scale and safe-area inset. */
export const screenHeaderTop =  (scale:number,inset:number) => Math.max(79*scale,inset+16);
/** Displays a centred screen title with an optional back button. */
export default function ScreenHeader({title,onBack,backLabel='Go back',scale=1,disabled=false,style}:{title:string;onBack?:()=>void;backLabel?:string;scale?:number;disabled?:boolean;style?:StyleProp<ViewStyle>}) {
 const [width,setWidth]=useState(0);
 // Explicit fitting also supports React Native Web, where adjustsFontSizeToFit is not implemented.
 /** Adds an estimated glyph width to the title-width calculation. */
 const units=[...title].reduce( (sum,char)=>sum+(' I1'.includes(char)?(char===' '?.3:.36):'MW'.includes(char)?.98:.74),0);
 const fontSize=width?Math.min(32*scale,(width-80*scale)/units):32*scale;
 return <View testID="screen-header" onLayout={
     /** Records the measured layout dimensions for this view. */
      event=>setWidth(event.nativeEvent.layout.width)} style={[s.header,{height:44*scale,minHeight:44*scale},style]}>
 <View pointerEvents="none" style={[StyleSheet.absoluteFill,{justifyContent:'center',paddingHorizontal:40*scale}]}><Text testID="screen-heading" accessibilityRole="header" numberOfLines={1} adjustsFontSizeToFit style={[s.title,{fontSize,lineHeight:40*scale}]}>{title}</Text></View>
 {!!onBack&&<Pressable accessibilityRole="button" accessibilityLabel={backLabel} disabled={disabled} onPress={onBack} style={[s.back,{width:44*scale,minWidth:44,height:44*scale,minHeight:44}]}><ChevronLeft size={32*scale} color="#e8b866"/></Pressable>}
 </View>;
}
const s=StyleSheet.create({header:{position:'relative',width:'100%',opacity:1,zIndex:2,justifyContent:'center'},title:{fontFamily:'Inter',fontWeight:'700',color:'#fff',opacity:1,textAlign:'center',includeFontPadding:false},back:{alignItems:'center',justifyContent:'center',zIndex:1}});
