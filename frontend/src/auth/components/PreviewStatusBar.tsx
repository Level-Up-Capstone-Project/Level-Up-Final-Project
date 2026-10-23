/** Displays a status-bar illustration for authentication previews. */
import { Platform, View } from 'react-native';
import { useAuthStyles } from '../styles';
import AuthIcon from './AuthIcon';
/** Displays a status-bar illustration for authentication previews. */
export default function PreviewStatusBar({ signup, signin }: {
    signup: boolean;
    signin: boolean;
}) {
    const styles = useAuthStyles();
    if (Platform.OS !== 'web')
        return null;
    return <View accessibilityElementsHidden importantForAccessibility="no-hide-descendants" style={styles.statusBar}>
    <AuthIcon name="time" style={styles.time}/>
    <View style={styles.statusIcons}>
      <AuthIcon name="alarm"/><AuthIcon name="bluetooth"/><AuthIcon name="wifi"/>
      <AuthIcon name={signup ? 'signupSignal' : signin ? 'signinSignal' : 'signal'} style={signup ? { marginTop: 0.507 } : undefined}/>
      <AuthIcon name={signup ? 'signupBattery' : signin ? 'signinBattery' : 'battery'} style={signup ? { marginTop: 0.507 } : undefined}/>
    </View>
  </View>;
}
