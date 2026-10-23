import type {BundledAsset} from '../platform/assets';
/** Displays bundled vector artwork in the web application. */
import { Image } from 'react-native';
/** Displays bundled vector artwork in the web application. */
export default function LocalVector({ source, width, height }: { source: BundledAsset; width: number; height: number }) {
    return <Image source={{ uri: source.uri }} style={{ width, height }} resizeMode="stretch"/>;
}
