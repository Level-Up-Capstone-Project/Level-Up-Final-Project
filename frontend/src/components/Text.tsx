/** Provides text with the application font and theme colour defaults. */
import { Text as NativeText, type TextProps } from 'react-native';
import { useTheme } from '../theme/ThemeContext';
/** Provides text with the application font and theme colour defaults. */
export default function Text({ style, ...props }: TextProps) {
    const { dark } = useTheme();
    return <NativeText {...props} style={[{ fontFamily: 'Inter', color: dark ? '#f4f1ea' : '#09090b' }, style]}/>;
}
