/** Displays the landscape inside the home mission card. */
import Svg, { Defs, LinearGradient, Rect, Stop } from 'react-native-svg';
import { Image, StyleSheet, View } from 'react-native';

// Figma 181:3920: full landscape image, 85% opacity, clipped to its frame.
/** Displays the landscape inside the home mission card. */
export default function MissionLandscape() {
    return <View pointerEvents="none" style={styles.frame}>
        <Image testID="home-mission-landscape" source={{uri:'/assets/home/next-mission.png'}} style={styles.image} resizeMode="stretch"/>
        <Svg testID="home-mission-vignette" width="100%" height="140.37%" style={StyleSheet.absoluteFill}><Defs><LinearGradient id="home-mission-shade" x1="0" y1="0" x2="0" y2="1"><Stop offset={0} stopColor="#000" stopOpacity={0}/><Stop offset={.50481} stopColor="#000" stopOpacity={.285}/><Stop offset={.75481} stopColor="#000" stopOpacity={.76}/><Stop offset={.87019} stopColor="#000" stopOpacity={.95}/><Stop offset={1} stopColor="#000" stopOpacity={.95}/></LinearGradient></Defs><Rect width="100%" height="100%" fill="url(#home-mission-shade)"/></Svg>
    </View>;
}
const styles = StyleSheet.create({
    frame:{...StyleSheet.absoluteFill,borderRadius:10,overflow:'hidden'},
    image:{width:'100%',height:'100%',opacity:.85},
});
