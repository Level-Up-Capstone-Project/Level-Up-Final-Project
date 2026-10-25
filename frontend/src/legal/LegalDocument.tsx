import type {BundledAsset} from '../platform/assets';
/** Displays a legal document with a fixed heading and scrollable body. */
import ScreenHeader, {screenHeaderTop} from '../components/ScreenHeader';
import { useState } from 'react';
import { Image, Pressable, ScrollView, StyleSheet, View, useWindowDimensions } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import Text from '../components/Text';
import LocalVector from '../onboarding/LocalVector';
import { legalDocuments } from './documents';
import type { LegalDocumentType } from './types';

/** Displays a legal document with a fixed heading and scrollable body. */
export default function LegalDocument({ document,onBack,onRelated }: { document:LegalDocumentType; onBack:()=>void; onRelated?:(document:LegalDocumentType)=>void }) {
    const {width}=useWindowDimensions();
    const insets=useSafeAreaInsets();
    const scale=Math.min(width,480)/408.393;
    const copy=legalDocuments[document];
    const [viewport,setViewport]=useState(0),[content,setContent]=useState(0),[offset,setOffset]=useState(0);
    const thumbHeight=content>0?Math.max(28,viewport*viewport/content):viewport;
    const thumbTop=content>viewport?Math.min(1,Math.max(0,offset/(content-viewport)))*(viewport-thumbHeight):0;
    /** Displays bundled vector artwork at the calculated screen scale. */
    const vector= (source:BundledAsset,w:number,h:number)=><View style={{width:w,height:h,transformOrigin:'top left',transform:[{scale}]}}><LocalVector source={source} width={w} height={h}/></View>;
    const related:LegalDocumentType=document==='terms'?'privacy':'terms';
    return <View style={s.safe}><View testID={'legal-'+document} style={s.canvas}>
        <View pointerEvents="none" style={StyleSheet.absoluteFill}>
            <View testID="legal-upper-contours" style={{position:'absolute',left:-26.78*scale,top:-36.18*scale}}>{vector({uri:'/assets/legal/current/c4099.svg'},462.8,198.328)}</View>
            <View testID="legal-lower-contours" style={{position:'absolute',left:-26.78*scale,bottom:-35.04*scale}}>{vector({uri:'/assets/legal/current/753f4.svg'},462.8,329.673)}</View>
        </View>
        <View style={{paddingTop:screenHeaderTop(scale,insets.top),paddingHorizontal:22*scale,paddingBottom:Math.max(24*scale,insets.bottom+8),flex:1}}>
            <ScreenHeader title={copy.title.toUpperCase()} onBack={onBack} backLabel={'Back from '+copy.title} scale={scale}/>
            <Text style={[s.kicker,{marginTop:17*scale,fontSize:Math.max(11,13*scale),lineHeight:16*scale}]}>{copy.kicker}</Text>
            <Text style={[s.description,{marginTop:20*scale,minHeight:40*scale,fontSize:Math.max(12,13*scale),lineHeight:Math.max(18,20*scale)}]}>{copy.description}</Text>
            <View testID="legal-document-card" style={[s.card,{marginTop:37*scale}]}>
                <View testID="legal-document-header" style={[s.documentHeader,{minHeight:78*scale,paddingHorizontal:16*scale,paddingVertical:14*scale,gap:14*scale}]}>
                    <Image testID="legal-document-icon" source={{uri:'/assets/settings/privacy.png'}} resizeMode="contain" style={{width:43*scale,height:43*scale}}/>
                    <View style={{flex:1}}><Text style={s.docTitle}>{copy.title}</Text><Text style={s.date}>Effective {copy.effectiveDate}</Text></View>
                </View>
                <View pointerEvents="none" style={{height:scale,overflow:'hidden'}}>{vector({uri:'/assets/legal/current/e23c2.svg'},360,1)}</View>
                <View style={{flex:1,minHeight:0}}>
                    <ScrollView testID="legal-document-scroll" showsVerticalScrollIndicator={false} onLayout={
                        /** Records the measured layout dimensions for this view. */
                         event=>setViewport(event.nativeEvent.layout.height)} onContentSizeChange={
                        /** Records the measured layout dimensions for this view. */
                         (_,h)=>setContent(h)} onScroll={
                        /** Records the document scroll position for the scrollbar. */
                         event=>setOffset(event.nativeEvent.contentOffset.y)} scrollEventThrottle={16} contentContainerStyle={{paddingHorizontal:7*scale,paddingTop:12*scale,paddingBottom:24*scale}}>
                        <Text testID="legal-lead" selectable style={[s.lead,{paddingHorizontal:9*scale,marginBottom:8*scale}]}>{copy.lead}</Text>
                        {copy.sections.map(
                            /** Renders a section entry from its data. */
                             (section,index)=><View testID="legal-section" key={section.heading}>
                            <View style={{paddingHorizontal:18*scale,paddingVertical:20*scale}}>
                                <Text accessibilityRole="header" style={s.sectionTitle}><Text style={s.number}>{String(index+1).padStart(2,'0')} </Text>{section.heading}</Text>
                                <Text testID="legal-section-body" selectable style={s.body}>{section.body}</Text>
                            </View>
                            {index<copy.sections.length-1&&<View testID="legal-section-divider" pointerEvents="none" style={{height:scale,overflow:'hidden'}}>{vector({uri:'/assets/legal/current/dbe0c.svg'},334,1)}</View>}
                        </View>)}
                        {onRelated && <Pressable accessibilityRole="link" accessibilityLabel={'Read '+legalDocuments[related].title} onPress={
                            /** Handles the on press for this component. */
                             ()=>onRelated(related)} style={[s.related,{marginHorizontal:9*scale,marginTop:12*scale}]}><Text style={s.relatedText}>Read {legalDocuments[related].title}</Text></Pressable>}
                    </ScrollView>
                    {content>viewport+1 && <View pointerEvents="none" testID="legal-scrollbar" style={s.scrollbar}><View testID="legal-scroll-thumb" style={{position:'absolute',top:thumbTop,width:3,height:thumbHeight,borderRadius:3,backgroundColor:'#a8773e'}}/></View>}
                </View>
            </View>
        </View>
    </View></View>;
}
const s=StyleSheet.create({
    safe:{flex:1,backgroundColor:'#000'},canvas:{flex:1,width:'100%',maxWidth:480,alignSelf:'center',overflow:'hidden'},
    header:{flexDirection:'row',alignItems:'center',minHeight:44,marginLeft:-10},back:{minHeight:44,minWidth:38,alignItems:'center',justifyContent:'center'},title:{fontWeight:'700',color:'#fff',flex:1},
    kicker:{fontWeight:'700',color:'#d4af53'},description:{color:'#a6a6a4'},
    card:{flex:1,minHeight:100,borderWidth:1,borderColor:'#353e44',borderRadius:10,backgroundColor:'rgba(6,15,20,.85)',overflow:'hidden'},documentHeader:{flexDirection:'row',alignItems:'center'},docTitle:{fontSize:13,lineHeight:20,fontWeight:'700',color:'#fff'},date:{fontSize:12,lineHeight:18,color:'#a6a6a4',marginTop:6},
    lead:{fontSize:13,lineHeight:20,color:'#a6a6a4'},sectionTitle:{fontSize:13,lineHeight:20,fontWeight:'700',color:'#fff',marginBottom:12},number:{color:'#d4af53'},body:{fontSize:13,lineHeight:20,color:'#a6a6a4'},related:{minHeight:44,justifyContent:'center'},relatedText:{fontSize:13,color:'#d4af53'},scrollbar:{position:'absolute',right:6,top:0,bottom:0,width:3,backgroundColor:'#14252b',borderRadius:3},
});
