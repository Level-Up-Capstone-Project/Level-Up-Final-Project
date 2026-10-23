/** Renders authentication icons in the web application. */
import { View, useWindowDimensions, type ViewStyle } from 'react-native';
import { SvgUri } from 'react-native-svg';
import { authAssets, type AuthAssetName } from '../assets';
/** Renders authentication icons in the web application. */
export default function AuthIcon({ name, style }: {
    name: AuthAssetName;
    style?: ViewStyle;
}) {
    const { width } = useWindowDimensions();
    const scale = Math.min(width, 480) / 408.393;
    const asset = authAssets[name];
    const loaded = asset.source;
    return <View pointerEvents="none" testID={`auth-asset-${name}`} style={[{ width: asset.width * scale, height: asset.height * scale }, style]}>
    <View style={{ width: asset.width, height: asset.height, transformOrigin: 'top left', transform: [{ scale }] }}><SvgUri uri={loaded.uri}/></View>
  </View>;
}
