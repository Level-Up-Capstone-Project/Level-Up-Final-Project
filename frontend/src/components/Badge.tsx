/** Displays a labelled skill badge with a proficiency status. */
import { useAppStyles } from './styles';
import Text from './Text';
/** Displays a labelled skill badge with a proficiency status. */
export default function Badge({ name, status }: {
    name: string;
    status: string;
}) {
    const s = useAppStyles();
    return <Text style={[s.badge, status === 'Have' ? s.have : status === 'Partial' ? s.partial : s.missing]}>{name}</Text>;
}
