import {BackHandler, Image, Platform, Pressable, ScrollView, StyleSheet, View} from 'react-native';
import {useEffect} from 'react';
import {useSafeAreaInsets} from 'react-native-safe-area-context';
import ScreenHeader, {screenHeaderTop} from '../components/ScreenHeader';
import Text from '../components/Text';
import OpportunityIcon from '../opportunities/OpportunityIcon';
import VignettePair from '../components/VignettePair';
import useResponsiveStyles from '../layout/useResponsiveStyles';
import type {AppModel} from '../navigation/useAppModel';

const cards = [
    {
        key:'intel',
        title:'INTEL',
        description:'Review your CV, transcript and skills insights to see where you stand.',
        image:'/assets/home/documents.png',
        destination:'analysis' as const,
        testID:'missions-intel-card',
    },
    {
        key:'missions',
        title:'MISSIONS',
        description:'Follow your personalized roadmap and build the skills for your next move.',
        image:'/assets/home/learning.png',
        destination:'learning' as const,
        testID:'missions-roadmap-card',
    },
];

export default function MissionHubScreen({model}:{model:AppModel}) {
    const styles=useResponsiveStyles(base);
    const insets=useSafeAreaInsets();
    const scale=Math.min(model.width,480)/408.393;
    useEffect(()=>{
        if(Platform.OS==='web')return;
        const back=BackHandler.addEventListener('hardwareBackPress',()=>{model.goBack('home');return true;});
        return ()=>back.remove();
    },[model.goBack]);
    return <View style={styles.safe}>
        <View style={styles.canvas}>
            <View pointerEvents="none" style={StyleSheet.absoluteFill}>
                <Image testID="mission-hub-landscape" source={{uri:'/assets/mission-hub/landscape.png'}}
                    resizeMode="cover" style={styles.background}/>
                <View style={styles.backgroundShade}/>
                <Image testID="mission-hub-upper-contours" source={{uri:'/assets/settings/upper-contours.svg'}}
                    resizeMode="stretch" style={[styles.upperContours,{width:414.8*scale,height:186.551*scale}]}/>
                <Image testID="mission-hub-lower-contours" source={{uri:'/assets/settings/lower-contours.svg'}}
                    resizeMode="stretch" style={[styles.lowerContours,{width:414.8*scale,height:310.065*scale}]}/>
                <VignettePair height={885.372*scale}/>
            </View>
            <ScrollView testID="mission-hub-content" contentContainerStyle={[styles.content,{
                paddingTop:screenHeaderTop(scale,insets.top),paddingBottom:110*scale+insets.bottom,
            }]}>
                <ScreenHeader title="MISSIONS" onBack={()=>model.goBack('home')}
                    backLabel="Back to previous page" scale={scale}/>
                <View style={styles.intro}>
                    <Text style={styles.eyebrow}>YOUR CAREER JOURNEY</Text>
                    <Text style={styles.heading}>Choose your next move</Text>
                    <Text style={styles.copy}>Explore your career intel or keep moving through your learning missions.</Text>
                </View>
                <View style={styles.cards}>
                    {cards.map(card=><Pressable key={card.key} testID={card.testID} accessibilityRole="button"
                        accessibilityLabel={`${card.title}. ${card.description}`} onPress={()=>model.go(card.destination)}
                        style={styles.card}>
                        <View style={styles.imageWrap}><Image source={{uri:card.image}} style={styles.image} resizeMode="contain"/></View>
                        <View style={styles.cardCopy}>
                            <Text style={styles.cardTitle}>{card.title}</Text>
                            <Text style={styles.cardDescription}>{card.description}</Text>
                            <View style={styles.open}><Text style={styles.openText}>OPEN {card.title}</Text><OpportunityIcon name="arrow" size={18*scale} color="#d4af53"/></View>
                        </View>
                    </Pressable>)}
                </View>
            </ScrollView>
        </View>
    </View>;
}

const base=StyleSheet.create({
    safe:{flex:1,backgroundColor:'#000'},
    canvas:{flex:1,width:'100%',maxWidth:480,alignSelf:'center',overflow:'hidden'},
    background:{position:'absolute',left:0,top:0,width:'100%',height:'100%'},
    backgroundShade:{...StyleSheet.absoluteFill,backgroundColor:'rgba(0,0,0,.56)'},
    upperContours:{position:'absolute',top:0,left:0,opacity:.42},
    lowerContours:{position:'absolute',bottom:0,left:0,opacity:.42},
    content:{flexGrow:1,paddingHorizontal:23},
    intro:{marginTop:52,marginBottom:26},
    eyebrow:{fontSize:12,fontWeight:'700',letterSpacing:1.4,color:'#d4af53'},
    heading:{fontSize:25,lineHeight:32,fontWeight:'700',color:'#fff',marginTop:10},
    copy:{fontSize:14,lineHeight:22,color:'#a6a6a4',marginTop:8},
    cards:{gap:16},
    card:{minHeight:178,padding:17,flexDirection:'row',alignItems:'center',gap:15,borderWidth:1,borderColor:'#69572f',borderRadius:14,backgroundColor:'rgba(9,15,18,.94)'},
    imageWrap:{width:74,height:74,alignItems:'center',justifyContent:'center'},
    image:{width:70,height:70},
    cardCopy:{flex:1},
    cardTitle:{fontSize:16,fontWeight:'700',letterSpacing:1,color:'#d4af53'},
    cardDescription:{fontSize:13,lineHeight:19,color:'#d3d3d0',marginTop:7},
    open:{flexDirection:'row',alignItems:'center',gap:8,marginTop:13},
    openText:{fontSize:11,fontWeight:'700',letterSpacing:.8,color:'#fff'},
});
