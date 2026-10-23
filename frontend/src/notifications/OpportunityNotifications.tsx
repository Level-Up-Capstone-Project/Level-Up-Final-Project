/** Displays opportunity alerts and their read/open actions. */
import { Modal, Pressable, ScrollView, StyleSheet, View } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import Text from '../components/Text';
import useResponsiveStyles from '../layout/useResponsiveStyles';
import type { OpportunityAlert } from './state';
/** Displays opportunity alerts and their read/open actions. */
export default function OpportunityNotifications({visible,onClose,alerts,unread,onOpen,onMarkAll,availableIds}:{visible:boolean;onClose:()=>void;alerts:OpportunityAlert[];unread:number;onOpen:(alert:OpportunityAlert)=>void;onMarkAll:()=>void;availableIds:string[]}) {
    const s=useResponsiveStyles(base);
    const insets=useSafeAreaInsets();
    return <Modal visible={visible} transparent animationType="slide" onRequestClose={onClose}><View style={s.scrim}><View style={[s.sheet,{paddingBottom:Math.max(20,insets.bottom)}]}>
        <View style={s.header}><View style={{flex:1}}><Text accessibilityRole="header" style={s.title}>Notifications</Text><Text style={s.subtitle}>New matches and opportunities</Text></View><Pressable accessibilityRole="button" accessibilityLabel="Close notifications" onPress={onClose} style={s.icon}><Text style={s.close}>✕</Text></Pressable></View>
        {!!alerts.length&&<Pressable accessibilityRole="button" accessibilityLabel="Mark all notifications as read" disabled={!unread} accessibilityState={{disabled:!unread}} onPress={onMarkAll} style={s.markAll}><Text style={[s.markText,!unread&&{color:'#777'}]}>Mark all as read</Text></Pressable>}
        <ScrollView contentContainerStyle={s.list}>
            {!alerts.length&&<View style={s.empty}><Text style={s.cardTitle}>No new matches yet</Text><Text style={s.copy}>New matches will appear here when opportunities are received.</Text></View>}
            {alerts.map(
                /** Renders a alert entry from its data. */
                 alert=>{const available=availableIds.includes(alert.opportunityId);return <Pressable key={alert.id} testID={`notification-${alert.id}`} accessibilityRole="button" accessibilityLabel={`${alert.read?'Read':'Unread'}: ${alert.title} at ${alert.company}`} onPress={
                /** Handles the on press for this component. */
                 ()=>onOpen(alert)} style={[s.card,!alert.read&&s.unread]}>
                <View style={s.kindRow}><Text style={s.kind}>{alert.kind==='new'?'NEW OPPORTUNITY MATCH':'STRONGER MATCH'}</Text>{!alert.read&&<View style={s.dot}/>}</View>
                <Text style={s.cardTitle}>{alert.title}</Text><Text style={s.copy}>{alert.company} · {alert.match}% match</Text>
                <Text style={s.received}>Received {new Date(alert.receivedAt).toLocaleString('en-ZA',{month:'short',day:'numeric',hour:'2-digit',minute:'2-digit'})}</Text>
                <Text style={s.action}>{available?'View opportunity →':'Opportunity currently unavailable'}</Text>
            </Pressable>})}
        </ScrollView>
    </View></View></Modal>;
}
const base=StyleSheet.create({scrim:{flex:1,backgroundColor:'rgba(0,0,0,.7)',justifyContent:'flex-end'},sheet:{width:'100%',maxWidth:480,alignSelf:'center',maxHeight:'85%',padding:20,borderTopLeftRadius:22,borderTopRightRadius:22,borderWidth:1,borderColor:'#353e44',backgroundColor:'#10191f'},header:{flexDirection:'row',alignItems:'center',gap:12},title:{fontSize:20,fontWeight:'700',color:'#e8b866'},subtitle:{fontSize:13,color:'#a6a6a4',marginTop:6},icon:{minWidth:44,minHeight:44,alignItems:'center',justifyContent:'center'},close:{fontSize:18,color:'#d4af53'},markAll:{minHeight:44,justifyContent:'center',alignSelf:'flex-end'},markText:{fontSize:12,color:'#d4af53'},list:{paddingTop:12,gap:12},empty:{padding:20,borderWidth:1,borderColor:'#353e44',borderRadius:10,gap:10},card:{padding:14,borderWidth:1,borderColor:'#353e44',borderRadius:10,backgroundColor:'#060f14'},unread:{borderColor:'#a58a39'},kindRow:{flexDirection:'row',alignItems:'center',justifyContent:'space-between',marginBottom:9},kind:{fontSize:11,fontWeight:'700',color:'#d4af53'},dot:{width:7,height:7,borderRadius:4,backgroundColor:'#d4af53'},cardTitle:{fontSize:14,fontWeight:'700',color:'#fff'},copy:{fontSize:13,lineHeight:19,color:'#a6a6a4',marginTop:6},received:{fontSize:11,color:'#7f8a93',marginTop:9},action:{fontSize:12,fontWeight:'600',color:'#d4af53',marginTop:10}});
