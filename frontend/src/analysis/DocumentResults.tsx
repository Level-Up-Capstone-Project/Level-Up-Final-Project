import {Pressable, StyleSheet, View} from 'react-native';
import Text from '../components/Text';
import type {TranscriptAnalysis} from '../api/transcripts';
import {useState} from 'react';

export function percentage(value: number | null | undefined): string {
    return typeof value === 'number' && Number.isFinite(value) && value >= 0 && value <= 100 ? `${Math.round(value * 100) / 100}%` : '—';
}

/** Shows the scores actually returned for each document, without combining unrelated measures. */
export default function DocumentResults({atsScore, formattingScore, transcript, onTranscript}: {
    atsScore?: number; formattingScore?: number; transcript: TranscriptAnalysis | null; onTranscript: () => void;
}) {
    const [showModules, setShowModules] = useState(false);
    const distinctions = transcript?.subjects?.filter(subject => Number.isFinite(subject.mark) && subject.mark >= 75 && subject.mark <= 100).length ?? 0;
    return <View style={s.results}>
        <View style={s.row}>
            <View><Text style={s.label}>CV ATS score</Text><Text testID="cv-ats-score" style={s.score}>{percentage(atsScore)}</Text></View>
            <View><Text style={s.label}>CV formatting score</Text><Text testID="cv-formatting-score" style={s.score}>{percentage(formattingScore)}</Text></View>
        </View>
        <View style={s.transcript}>
            <Text style={s.heading}>Step 2 · Transcript analysis</Text>
            {!!transcript?.institution && <Text style={s.copy}>{transcript.institution}</Text>}
            {!!transcript?.qualification && <Text style={s.copy}>{transcript.qualification}</Text>}
            {transcript
                ? <View testID="transcript-academic-summary" style={s.metricsRow}>
                    <View style={s.metricCard}>
                        <Text style={s.metricLabel}>AVERAGE MARK</Text>
                        <Text testID="transcript-average-mark" style={s.metricScore}>{percentage(transcript.averageMark)}</Text>
                        <Text style={s.metricCaption}>Across your modules</Text>
                    </View>
                    <View style={s.metricCard}>
                        <Text style={s.metricLabel}>DISTINCTIONS</Text>
                        <View style={s.distinctionValue}>
                            <Text testID="transcript-distinction-count" style={s.metricScore}>{distinctions}</Text>
                            <Text style={s.metricThreshold}>75%+</Text>
                        </View>
                        <Text style={s.metricCaption}>Modules passed with distinction</Text>
                    </View>
                </View>
                : <Text style={s.copy}>Add your transcript to see your academic results.</Text>}
            {!!transcript?.strengths?.length && <><Text style={s.heading}>Highlights</Text>{transcript.strengths.slice(0, 3).map((strength,index)=><Text key={`strength-${index}`} style={s.copy}>• {strength}</Text>)}</>}
            {!!transcript?.subjects?.length && <Pressable accessibilityRole="button" accessibilityState={{expanded:showModules}} onPress={() => setShowModules(value => !value)} style={s.action}><Text style={s.heading}>{showModules ? 'Hide module marks' : `View module marks (${transcript.subjects.length})`}</Text></Pressable>}
            {showModules && transcript?.subjects.map((subject, index) => <View key={`${subject.subject}-${index}`} style={s.subject}>
                <Text style={[s.copy, {flex: 1}]}>{subject.subject}</Text><Text style={s.copy}>{percentage(subject.mark)}</Text>
            </View>)}
            {transcript && <Text style={s.copy}>Check extracted marks against your transcript.</Text>}
            <Pressable accessibilityRole="button" accessibilityLabel={transcript ? 'Replace transcript' : 'Add transcript'} onPress={onTranscript} style={s.action}>
                <Text style={s.heading}>{transcript ? 'Replace transcript' : 'Add transcript'}</Text>
            </Pressable>
        </View>
    </View>;
}

const s = StyleSheet.create({
    results: {marginTop: 14, gap: 14},
    row: {flexDirection: 'row', justifyContent: 'space-between', gap: 16, padding: 16, borderWidth: 1, borderColor: '#353e44', borderRadius: 10, backgroundColor: 'rgba(6,15,20,.78)'},
    transcript: {padding: 16, gap: 8, borderWidth: 1, borderColor: '#353e44', borderRadius: 10, backgroundColor: 'rgba(6,15,20,.78)'},
    heading: {fontSize: 13, lineHeight: 20, color: '#d4af53', fontWeight: '700'},
    label: {fontSize: 12, lineHeight: 18, color: '#d1d1d6'},
    score: {fontSize: 20, lineHeight: 28, color: '#fff', fontWeight: '700'},
    copy: {fontSize: 12, lineHeight: 19, color: '#d1d1d6'},
    metricsRow: {flexDirection: 'row', gap: 8, marginTop: 2},
    metricCard: {flex: 1, minWidth: 0, minHeight: 88, justifyContent: 'center', padding: 10, borderRadius: 8, borderWidth: 1, borderColor: 'rgba(212,175,83,.28)', backgroundColor: 'rgba(212,175,83,.055)'},
    metricLabel: {fontSize: 10, lineHeight: 15, letterSpacing: .7, fontWeight: '700', color: '#c6a76a'},
    metricScore: {fontSize: 22, lineHeight: 28, fontWeight: '700', color: '#fff'},
    metricCaption: {fontSize: 10, lineHeight: 14, color: '#aaa9a4'},
    distinctionValue: {flexDirection: 'row', alignItems: 'baseline', gap: 6},
    metricThreshold: {fontSize: 11, lineHeight: 16, fontWeight: '600', color: '#d4af53'},
    subject: {flexDirection: 'row', gap: 12, justifyContent: 'space-between'},
    action: {minHeight: 44, justifyContent: 'center'},
});
