/** Provides the shared card container used by application content. */
import { Pressable, View } from 'react-native';
import { useAppStyles } from './styles';
import Text from './Text';
/** Provides the shared card container used by application content. */
export default function Card({ title, subtitle, onPress }: {
    title: string;
    subtitle?: string;
    onPress?: () => void;
}) {
    const s = useAppStyles();
    return <Pressable accessibilityRole={onPress ? 'button' : undefined} onPress={onPress} style={s.card}><View style={s.flex}><Text style={s.bold}>{title}</Text>{subtitle && <Text style={s.small}>{subtitle}</Text>}</View>{onPress && <Text style={s.cardArrow}>›</Text>}</Pressable>;
}
