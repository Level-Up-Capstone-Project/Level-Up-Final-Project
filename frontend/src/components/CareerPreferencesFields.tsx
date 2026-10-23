/** Collects an industry and an editable list of possible target roles. */
import {useState} from 'react';
import {Pressable,StyleSheet,TextInput,View} from 'react-native';
import Text from './Text';
/** Collects an industry and an editable list of possible target roles. */
export default function CareerPreferencesFields({industry,onIndustryChange,targetRoles,onRolesChange}:{industry:string;onIndustryChange:(value:string)=>void;targetRoles:string[];onRolesChange:(value:string[])=>void}) {
 const [draft,setDraft]=useState(''),[focused,setFocused]=useState('');
 /** Adds a non-empty, unique target role to the selected roles. */
 const add= ()=>{const value=draft.trim();if(!value)return;if(!targetRoles.some(
     /** Checks the role against the current selection or criteria. */
      role=>role.toLowerCase()===value.toLowerCase()))onRolesChange([...targetRoles,value]);setDraft('');};
 return <View style={s.container}>
 <Text style={s.label}>Industry</Text><TextInput accessibilityLabel="Industry" placeholder="Enter your industry" placeholderTextColor="#7c8ba0" value={industry} onChangeText={onIndustryChange} maxLength={120} autoCapitalize="words" onFocus={
     /** Updates input focus and its visual state. */
      ()=>setFocused('industry')} onBlur={
     /** Updates input focus and its visual state. */
      ()=>setFocused('')} style={[s.input,focused==='industry'&&s.focused]}/>
 <Text style={s.label}>Possible target roles</Text><Text style={s.hint}>Add one or more roles you would consider.</Text>
 <View style={s.row}><TextInput accessibilityLabel="Add target role" placeholder="Enter a target role" placeholderTextColor="#7c8ba0" value={draft} onChangeText={setDraft} maxLength={120} autoCapitalize="words" returnKeyType="done" onSubmitEditing={add} onFocus={
     /** Updates input focus and its visual state. */
      ()=>setFocused('role')} onBlur={
     /** Updates input focus and its visual state. */
      ()=>setFocused('')} style={[s.input,{flex:1},focused==='role'&&s.focused]}/><Pressable accessibilityRole="button" accessibilityLabel="Add target role" disabled={!draft.trim()} onPress={add} style={[s.add,!draft.trim()&&{opacity:.5}]}><Text style={s.addText}>Add</Text></Pressable></View>
 <View style={s.chips}>{targetRoles.map(
     /** Renders a role entry from its data. */
      role=><Pressable key={role.toLowerCase()} accessibilityRole="button" accessibilityLabel={'Remove target role: '+role} onPress={
     /** Handles the on press for this component. */
      ()=>onRolesChange(targetRoles.filter(
     /** Selects the roles that match the current conditions. */
      item=>item!==role))} style={s.chip}><Text style={s.chipText}>{role}  ×</Text></Pressable>)}</View>
 </View>;
}
const s=StyleSheet.create({container:{gap:8,marginVertical:12},label:{fontSize:14,color:'#fff',marginTop:4},hint:{fontSize:12,color:'#a6a6a4',lineHeight:18},input:{minHeight:52,borderWidth:1,borderColor:'#353e44',borderRadius:10,padding:14,backgroundColor:'#060f14',color:'#fff',fontSize:14},focused:{borderColor:'#d4af53'},row:{flexDirection:'row',gap:8,alignItems:'center'},add:{minHeight:52,paddingHorizontal:16,borderRadius:10,backgroundColor:'#d4af53',justifyContent:'center'},addText:{color:'#000',fontWeight:'700'},chips:{flexDirection:'row',flexWrap:'wrap',gap:8},chip:{padding:10,borderRadius:8,borderWidth:1,borderColor:'#765728',backgroundColor:'#252112',minHeight:44,justifyContent:'center'},chipText:{color:'#d4af53',fontSize:13,flexShrink:1}});
