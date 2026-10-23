import Text from '../components/Text';
import type {TextStyle, StyleProp} from 'react-native';

/** Renders the common Markdown formatting used by MIRA without interpreting HTML. */
export default function MessageText({text, style}: {text: string; style?: StyleProp<TextStyle>}) {
    const lines = text.replace(/\r\n?/g, '\n').split('\n');
    return <Text selectable accessibilityLiveRegion="polite" style={style}>{lines.map((line, lineIndex) => {
        const display = line.replace(/^\s*[-*]\s+/, '• ').replace(/^#{1,6}\s+/, '');
        const parts = display.split(/(\*\*[^*\n]+\*\*)/g);
        return <Text key={lineIndex}>{parts.map((part, index) => part.startsWith('**') && part.endsWith('**')
            ? <Text key={index} style={{fontWeight: '700'}}>{part.slice(2, -2)}</Text>
            : part)}{lineIndex < lines.length - 1 ? '\n' : ''}</Text>;
    })}</Text>;
}
