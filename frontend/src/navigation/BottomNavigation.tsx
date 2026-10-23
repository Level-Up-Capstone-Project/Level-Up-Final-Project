/** Displays the bottom navigation tabs and their active state. */
import { Image, Pressable, StyleSheet, useWindowDimensions, View } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import Text from '../components/Text';
import OpportunityIcon from '../opportunities/OpportunityIcon';
import useResponsiveStyles from '../layout/useResponsiveStyles';
import type { Screen } from './types';

const tabs = [['mira','mira','MIRA'],['missionHub','missions','Missions'],['home','home','Home'],['achievements','medals','Medals'],['settings','menu','MENU']] as const;
/** Displays the bottom navigation tabs and their active state. */
export default function BottomNavigation({ screen, go, inline = false }: { screen: Screen; go: (screen: Screen) => void; inline?: boolean }) {
    const s = useResponsiveStyles(styles);
    const insets = useSafeAreaInsets();
    const { width } = useWindowDimensions();
    const active = screen === 'profileSettings' ? 'settings' : screen === 'job' ? 'jobs' : screen === 'complete' ? 'achievements' : ['topic','learning','analysis','skills'].includes(screen) ? 'missionHub' : screen;
    return <View style={[s.nav, inline && { position:'relative',flexShrink:0 }, { paddingBottom: Math.max(10, insets.bottom) }]}>{tabs.map(
        /** Renders a item entry from its data. */
         ([route, icon, label]) => {
        const selected = active === route;
        return <Pressable key={route} accessibilityRole="tab" accessibilityLabel={label} accessibilityState={{ selected }} aria-selected={selected} onPress={
            /** Handles the on press for this component. */
             () => go(route)} style={s.item}>
            <View style={[s.icon, selected && s.selected]}>{(icon === 'mira' || icon === 'medals') ? <Image testID={icon === 'mira' ? 'navbar-mira-icon' : 'navbar-medals-icon'} accessible={false} source={icon === 'mira' ? {uri:'/assets/navigation/mira.png'} : {uri:'/assets/navigation/medals.png'}} resizeMode="cover" style={{ tintColor:icon === 'medals' && selected ? '#000' : undefined, width:(icon === 'medals' ? 33 : 34) * Math.min(width,480) / 408.393, height:(icon === 'medals' ? 33 : 34) * Math.min(width,480) / 408.393 }}/> : <OpportunityIcon name={icon} size={(icon === 'menu' ? 32 : 28) * Math.min(width,480) / 408.393} color={selected ? '#000' : '#a6a6a4'}/>}</View>
            <Text style={s.text}>{label}</Text>
        </Pressable>;
    })}</View>;
}
const styles = StyleSheet.create({
    nav:{position:'absolute',bottom:0,left:0,right:0,paddingTop:7,paddingHorizontal:12,backgroundColor:'rgba(0,0,0,.94)',borderTopWidth:1,borderColor:'#a58a39',flexDirection:'row',justifyContent:'space-around'},
    item:{flex:1,minHeight:64,alignItems:'center',justifyContent:'center'},icon:{minWidth:44,minHeight:44,borderRadius:10,alignItems:'center',justifyContent:'center'},selected:{backgroundColor:'#d4af53'},text:{color:'#a6a6a4',fontSize:13,marginTop:4},
});
