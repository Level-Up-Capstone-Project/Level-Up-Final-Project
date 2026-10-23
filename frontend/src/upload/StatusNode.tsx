/** Displays the state of an individual analysis stage. */
import UploadVector from './UploadVector';
import { ActivityIndicator } from 'react-native';
import type { AnalysisTaskStatus } from './analysisState';
/** Displays the state of an individual analysis stage. */
export default function StatusNode({ state, size = 24, gold = false }: { state: AnalysisTaskStatus; size?: number; gold?:boolean }) {
    if (state === 'processing') return <ActivityIndicator accessibilityLabel="Processing" color="#d4af53" size="small" style={{ width: size, height: size }} />;
    const source = state === 'completed' ? (gold ? {uri:'/assets/upload/b4b2b.svg'} : {uri:'/assets/upload/50080.svg'}) : state === 'failed' ? {uri:'/assets/upload/b791d.svg'} : {uri:'/assets/upload/ec8da.svg'};
    return <UploadVector source={source} width={state==='completed'&&gold?15:24} size={size}/>;
}
