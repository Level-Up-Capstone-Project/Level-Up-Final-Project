/** Provides searchable work-area selection with selected values inside the field. */
import { useEffect, useRef, useState } from 'react';
import { ActivityIndicator, Pressable, ScrollView, StyleSheet, TextInput, View } from 'react-native';
import Text from '../components/Text';
import { createSessionToken, placesConfigured, searchAreas, selectArea, type AreaSuggestion } from './places';
import { southAfrica, provinces, type WorkArea } from './types';

/** Provides searchable work-area selection with selected values inside the field. */
export default function WorkAreaPicker({ value, onChange }: { value:WorkArea[]; onChange:(areas:WorkArea[])=>void }) {
    const [open,setOpen]=useState(false), [query,setQuery]=useState(''), [results,setResults]=useState<AreaSuggestion[]>([]), [error,setError]=useState(''), [loading,setLoading]=useState(false), [selecting,setSelecting]=useState(false);
    const [draft,setDraft]=useState(value);
    const token=useRef(createSessionToken()), version=useRef(0), mounted=useRef(true);
    useEffect(
        /** Synchronises component state with the effect dependencies. */
         ()=>{mounted.current=true;
    /** Runs the effect cleanup when its dependencies change or the component closes. */
    return  ()=>{mounted.current=false;version.current++;};},[]);
    useEffect(
        /** Runs the current search and cancels superseded requests. */
         ()=>{
        const controller=new AbortController(); const current=++version.current;
        setResults([]);setError('');setLoading(false);
        if(!open || query.trim().length<2 || !placesConfigured)
        /** Cancels outstanding work when the effect ends. */
        return  ()=>controller.abort();
        setLoading(true);
        /** Requests area suggestions after the search-input delay. */
        const timer=setTimeout( ()=>{searchAreas(query.trim(),token.current,controller.signal).then(
            /** Stores the successful result of the asynchronous operation. */
             items=>{if(current===version.current)setResults(items);}).catch(
            /** Displays or records the error from the asynchronous operation. */
             e=>{if(!controller.signal.aborted && current===version.current)setError(e.message);}).finally(
            /** Clears the operation pending state after it finishes. */
             ()=>{if(current===version.current)setLoading(false);});},300);
        /** Cancels outstanding work when the effect ends. */
        return  ()=>{clearTimeout(timer);controller.abort();};
    },[query,open]);
    /** Closes the area picker when selection is not in progress. */
    const close= ()=>{if(!selecting){version.current++;setOpen(false);}};
    /** Toggles a work area within the pending selection. */
    const add= (area:WorkArea)=>{setDraft(
        /** Builds the next draft value from its previous state. */
         old=>old.some(
        /** Checks the item against the current selection or criteria. */
         item=>item.placeId===area.placeId)?old.filter(
        /** Selects the item that match the current conditions. */
         item=>item.placeId!==area.placeId):area.kind==='country' ? [area] : [...old.filter(
        /** Selects the item that match the current conditions. */
         item=>item.kind!=='country' && item.placeId!==area.placeId),area]);setQuery('');setResults([]);token.current=createSessionToken();};
    /** Resolves an area suggestion and adds it to the pending selection. */
    const choose= async (suggestion:AreaSuggestion)=>{
        if(selecting)return;setSelecting(true);setError('');const current=++version.current;
        try{const area=await selectArea(suggestion.placeId,token.current);if(mounted.current && current===version.current)add(area);}
        catch(e){if(mounted.current && current===version.current)setError(e instanceof Error?e.message:'Unable to select this area.');}
        finally{if(mounted.current)setSelecting(false);}
    };
    const displayed=open?draft:value;
    /** Checks whether an area is in the pending selection. */
    const selected= (area:WorkArea)=>draft.some(
        /** Checks the item against the current selection or criteria. */
         item=>item.placeId===area.placeId);
    return <View>
        <Pressable accessibilityRole="button" accessibilityLabel="Choose preferred work areas" accessibilityState={{expanded:open}} accessibilityValue={{text:displayed.map(
            /** Transforms each item into the required display or state value. */
             area=>area.label).join('; ')}} onPress={
            /** Handles the on press for this component. */
             ()=>{if(open){close();return;}setDraft(value);setQuery('');setError('');token.current=createSessionToken();setOpen(true);}} style={s.field}><Text testID="work-area-field-value" numberOfLines={1} ellipsizeMode="tail" style={[s.fieldText,{color:displayed.length?'#262626':'#7c8ba0'}]}>{displayed.length ? displayed.map(
            /** Transforms each item into the required display or state value. */
             area=>area.label).join('; ') : 'Preferred work areas'}</Text><Text style={s.gold}>{open ? '⌃' : '⌄'}</Text></Pressable>
        {open && <View testID="work-area-dropdown" style={s.container}>
                <Text style={s.help}>Choose work areas, not a street address. Tap a selected area to remove it.</Text>
                <TextInput accessibilityLabel="Search South African areas" placeholder="Search an area in South Africa" placeholderTextColor="#7c8ba0" value={query} maxLength={150} onChangeText={setQuery} editable={!selecting} autoCapitalize="words" style={s.search}/>
                <ScrollView nestedScrollEnabled keyboardShouldPersistTaps="handled" style={{maxHeight:180}} contentContainerStyle={{paddingVertical:12,gap:10}}>

                    <Pressable accessibilityRole="checkbox" accessibilityLabel="Anywhere in South Africa" accessibilityState={{checked:selected(southAfrica),disabled:selecting}} disabled={selecting} onPress={
                        /** Handles the on press for this component. */
                         ()=>add(southAfrica)} style={[s.option,selected(southAfrica)&&s.selectedOption]}><Text style={s.copy}>Anywhere in South Africa</Text>{selected(southAfrica)&&<Text style={s.check}>✓</Text>}</Pressable>
                    {provinces.filter(
                        /** Selects the provinces that match the current conditions. */
                         area=>!query.trim() || area.label.toLowerCase().includes(query.trim().toLowerCase())).map(
                        /** Renders a province entry from its data. */
                         area=><Pressable key={area.placeId} accessibilityRole="checkbox" accessibilityLabel={area.label} accessibilityState={{checked:selected(area),disabled:selecting}} disabled={selecting} onPress={
                        /** Handles the on press for this component. */
                         ()=>add(area)} style={[s.option,selected(area)&&s.selectedOption]}><Text style={s.copy}>{area.label}</Text>{selected(area)&&<Text style={s.check}>✓</Text>}</Pressable>)}
                    {!placesConfigured && <Text style={s.help}>Town and suburb search will be available when area search is connected.</Text>}
                    {(loading || selecting) && <ActivityIndicator accessibilityLabel={selecting?'Selecting work area':'Searching areas'} color="#d4af53"/>}
                    {!!error && <Text accessibilityRole="alert" style={s.help}>{error}</Text>}
                    {results.length>0 && <View style={s.results}>{results.map(
                        /** Renders a item entry from its data. */
                         item=><Pressable key={item.placeId} accessibilityRole="button" disabled={selecting} onPress={
                        /** Handles the on press for this component. */
                         ()=>choose(item)} style={s.option}><Text style={s.copy}>{item.label}</Text></Pressable>)}<Text style={s.attribution}>Google Maps</Text></View>}
                    {placesConfigured && query.trim().length>=2 && !loading && !selecting && !error && !results.length && <Text style={s.help}>No matching areas found.</Text>}
                </ScrollView>
                <Pressable accessibilityRole="button" accessibilityLabel="Use selected work areas" disabled={!draft.length || selecting} onPress={
                    /** Applies the selected work areas and closes the picker. */
                     ()=>{onChange(draft);setOpen(false);}} style={[s.done,(!draft.length || selecting)&&{opacity:.5}]}><Text style={s.doneText}>Use selected areas</Text></Pressable>
                <Pressable accessibilityRole="button" onPress={close} disabled={selecting} style={s.cancel}><Text style={s.copy}>Cancel</Text></Pressable>
        </View>}
    </View>;
}
const s=StyleSheet.create({
    field:{minHeight:60,paddingHorizontal:23,paddingVertical:12,borderRadius:14,backgroundColor:'#f5f9fe',flexDirection:'row',alignItems:'center',gap:12},fieldText:{fontFamily:'Poppins',fontSize:16,flex:1},gold:{color:'#d4af53',fontSize:24},
    container:{width:'100%',backgroundColor:'#0b151c',borderWidth:1,borderColor:'#d4af53',borderRadius:14,padding:12,marginTop:6},help:{fontSize:13,color:'#a6a6a4',lineHeight:22,marginBottom:14},
    search:{minHeight:56,borderWidth:1,borderColor:'#d4af53',borderRadius:14,backgroundColor:'#f5f9fe',padding:16,fontSize:16,color:'#262626'},copy:{fontSize:13,color:'#fff',flexShrink:1},option:{minHeight:48,padding:12,borderWidth:1,borderColor:'#353e44',borderRadius:10,flexDirection:'row',alignItems:'center',justifyContent:'space-between',gap:8},selectedOption:{backgroundColor:'#252112',borderColor:'#765728'},check:{color:'#d4af53',fontSize:16},chip:{flexDirection:'row',alignItems:'center',justifyContent:'space-between',paddingLeft:14,borderRadius:10,backgroundColor:'#252112'},remove:{minHeight:44,minWidth:44,alignItems:'center',justifyContent:'center'},results:{gap:8,borderWidth:1,borderColor:'#353e44',borderRadius:12,padding:10},attribution:{fontSize:12,color:'#fff',padding:10},done:{minHeight:50,backgroundColor:'#d4af53',borderRadius:10,justifyContent:'center',alignItems:'center'},doneText:{fontSize:14,fontWeight:'700',color:'#000'},cancel:{minHeight:44,alignItems:'center',justifyContent:'center',marginTop:8},
});
