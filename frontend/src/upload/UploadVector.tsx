import type {BundledAsset} from '../platform/assets';
/** Displays bundled vector artwork used by upload controls. */
import { View } from 'react-native';
import { SvgUri } from 'react-native-svg';

// Keep SVG root dimensions intact and scale their containing view.
/** Displays bundled vector artwork used by upload controls. */
export default function UploadVector({ source, width, height = width, size = width }: { source: BundledAsset; width: number; height?: number; size?: number }) {
    const asset = source;
    return <View pointerEvents="none" style={{ width: size, height: height * size / width }}><View style={{ width, height, transformOrigin: 'top left', transform: [{ scale: size / width }] }}><SvgUri uri={asset.uri}/></View></View>;
}
