/** Displays text or structured CV feedback as a readable report. */
import { StyleSheet, View } from 'react-native';
import Text from '../components/Text';
import type { CvFeedback } from './feedback';

type Block = {kind:'paragraph'|'heading'|'bullet'|'numbered';text:string;marker?:string};
/** Splits report text into paragraphs, headings and list blocks. */
function blocks(text:string):Block[] {
    const result:Block[]=[];
    let paragraph:string[]=[];
    /** Adds the accumulated paragraph lines to the report blocks. */
    const flush= ()=>{if(paragraph.length)result.push({kind:'paragraph',text:paragraph.join(' ')});paragraph=[];};
    for(const line of text.replace(/\r\n?/g,'\n').split('\n')) {
        const heading=line.match(/^\s*#{1,6}\s+(.+)$/);
        const bullet=line.match(/^\s*[-*•]\s+(.+)$/);
        const numbered=line.match(/^\s*(\d+)[.)]\s+(.+)$/);
        if(!line.trim()){flush();continue;}
        if(heading){flush();result.push({kind:'heading',text:heading[1].replace(/\s+#+\s*$/,'')});}
        else if(bullet){flush();result.push({kind:'bullet',text:bullet[1],marker:'•'});}
        else if(numbered){flush();result.push({kind:'numbered',text:numbered[2],marker:numbered[1]+'.'});}
        else paragraph.push(line.trim());
    }
    flush();return result;
}
/** Displays inline emphasis within report text. */
function Inline({text}:{text:string}) {
    return <>{text.split(/(\*\*[^*]+\*\*)/g).map(
        /** Renders a item entry from its data. */
         (part,index)=>part.startsWith('**')&&part.endsWith('**')?<Text key={index} style={s.bold}>{part.slice(2,-2)}</Text>:part)}</>;
}
/** Displays formatted paragraphs and lists from a report string. */
function Prose({text}:{text:string}) {
    return <>{blocks(text).map(
        /** Renders a item entry from its data. */
         (block,index)=>block.kind==='heading'?<Text key={index} selectable accessibilityRole="header" style={s.section}><Inline text={block.text}/></Text>:block.marker?<View key={index} style={s.listRow}><Text style={s.marker}>{block.marker}</Text><Text selectable style={[s.body,s.listBody]}><Inline text={block.text}/></Text></View>:<Text key={index} selectable style={s.body}><Inline text={block.text}/></Text>)}</>;
}
/** Displays text or structured CV feedback as a readable report. */
export default function CvFeedbackReport({feedback}:{feedback?:CvFeedback}) {
    const available=typeof feedback==='string'?!!feedback.trim():!!feedback&&(!!feedback.introduction?.trim()||!!feedback.conclusion?.trim()||feedback.sections.some(
        /** Checks the section against the current selection or criteria. */
         section=>!!section.body?.trim()||!!section.bullets?.some(
        /** Checks the item against the current selection or criteria. */
         item=>item.trim())));
    return <View testID="cv-feedback-report" style={s.card}>
        <Text accessibilityRole="header" style={s.title}>{typeof feedback==='object'&&feedback.title?.trim()?feedback.title:'Detailed Feedback'}</Text>
        {!available?<Text style={s.body}>Detailed feedback will appear here when it is provided by the analysis service.</Text>:typeof feedback==='string'?<Prose text={feedback}/>:feedback&&<>
            {!!feedback.introduction?.trim()&&<Prose text={feedback.introduction}/>}
            {feedback.sections.map(
                /** Renders a section entry from its data. */
                 (section,index)=><View key={index}><Text selectable accessibilityRole="header" style={s.section}>{section.heading}</Text>{!!section.body?.trim()&&<Prose text={section.body}/>}{section.bullets?.filter(
                /** Selects the item that match the current conditions. */
                 item=>item.trim()).map(
                /** Renders a item entry from its data. */
                 (item,i)=><View key={i} style={s.listRow}><Text style={s.marker}>•</Text><Text selectable style={[s.body,s.listBody]}><Inline text={item}/></Text></View>)}</View>)}
            {!!feedback.conclusion?.trim()&&<><Text accessibilityRole="header" style={s.section}>Conclusion</Text><Prose text={feedback.conclusion}/></>}
        </>}
    </View>;
}
const s=StyleSheet.create({
    card:{borderWidth:1,borderColor:'#353e44',borderRadius:10,backgroundColor:'rgba(6,15,20,.85)',padding:18,marginTop:4,marginBottom:18},
    title:{fontSize:17,lineHeight:24,fontWeight:'700',color:'#fff',marginBottom:14},
    section:{fontSize:15,lineHeight:23,fontWeight:'700',color:'#d4af53',marginTop:12,marginBottom:10},
    body:{fontSize:14,lineHeight:23,color:'#d1d1d6',marginBottom:12,flexShrink:1},bold:{fontWeight:'700',color:'#fff'},
    listRow:{flexDirection:'row',gap:9,alignItems:'flex-start'},marker:{color:'#d4af53',fontSize:14,lineHeight:23,minWidth:14},listBody:{flex:1},
});
