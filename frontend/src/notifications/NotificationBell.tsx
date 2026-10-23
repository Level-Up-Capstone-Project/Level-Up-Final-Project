/** Displays a notification bell and its unread count. */
import { Pressable, StyleSheet, View } from 'react-native';
import Text from '../components/Text';
import OpportunityIcon from '../opportunities/OpportunityIcon';
/** Displays a notification bell and its unread count. */
export default function NotificationBell({unread,onPress}:{unread:number;onPress:()=>void}) {
    return <Pressable accessibilityRole="button" accessibilityLabel="Opportunity notifications" accessibilityHint={unread?`${unread} unread notifications`:'No unread notifications'} onPress={onPress} style={s.button}>
        <OpportunityIcon name="bell" size={24} color="#9d813b"/>
        {unread>0&&<View style={s.badge}><Text testID="opportunity-unread-count" accessibilityLiveRegion="polite" style={s.count}>{unread>99?'99+':unread}</Text></View>}
    </Pressable>;
}
const s=StyleSheet.create({button:{minWidth:44,minHeight:44,alignItems:'center',justifyContent:'center'},badge:{position:'absolute',right:0,top:0,minWidth:17,minHeight:17,borderRadius:10,backgroundColor:'#d4af53',borderWidth:1,borderColor:'#000',paddingHorizontal:3,justifyContent:'center',alignItems:'center'},count:{fontSize:10,fontWeight:'700',color:'#000'}});
