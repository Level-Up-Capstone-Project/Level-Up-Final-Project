/** Provides a reusable application button. */
import React from 'react';
import { Pressable } from 'react-native';
import { useAppStyles } from './styles';
import Text from './Text';
/** Provides a reusable application button. */
export default function Button({ children, onPress, outline = false }: {
    children: React.ReactNode;
    onPress: () => void;
    outline?: boolean;
}) {
    const s = useAppStyles();
    return <Pressable accessibilityRole="button" onPress={onPress} style={
        /** Returns the control styles for its current pressed state. */
         ({ pressed }) => [s.button, outline && s.outline, pressed && { opacity: 0.65 }]}><Text style={[s.buttonText, outline && s.dark]}>{children}</Text></Pressable>;
}
