/** Renders an icon from the bundled SVG collection. */
import { SvgXml } from 'react-native-svg';
import { icons } from '../icons';
/** Renders an icon from the bundled SVG collection. */
export default function Icon({ name, size = 24, active = false }: {
    name: string;
    size?: number;
    active?: boolean;
}) {
    const xml = active ? icons[name].replace(/#(?:[a-f0-9]{6}|[a-f0-9]{3})\b/gi, '#000000') : icons[name];
    return <SvgXml xml={xml} width={size} height={size}/>;
}
