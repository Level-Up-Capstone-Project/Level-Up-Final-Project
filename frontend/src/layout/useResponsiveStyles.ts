/** Scales layout styles for the current phone width. */
import { useMemo } from 'react';
import { useWindowDimensions } from 'react-native';

// Scale layout dimensions rather than transforming an interactive screen.
// Text can still wrap and native accessibility font scaling stays enabled.
const dimensions = /^(width|height|minWidth|minHeight|padding.*|margin.*|gap|rowGap|columnGap|borderRadius|fontSize|lineHeight|letterSpacing|top|bottom|left|right)$/;
/** Scales layout styles for the current phone width. */
export default function useResponsiveStyles<T extends Record<string, object>>(styles: T, referenceWidth = 408.393): T {
    const { width } = useWindowDimensions();
    const scale = Math.min(width, 480) / referenceWidth;
    return useMemo(
        /** Calculates the derived values used by the current view. */
         () => Object.fromEntries(Object.entries(styles).map(
        /** Transforms each item into the required display or state value. */
         ([name, style]) => [name,
        Object.fromEntries(Object.entries(style).map(
            /** Transforms each item into the required display or state value. */
             ([key, value]) => {
            if (typeof value !== 'number' || !dimensions.test(key)) return [key, value];
            if (key === 'fontSize') return [key, Math.max(11, value * scale)];
            if (key === 'lineHeight') return [key, Math.max(16, value * scale)];
            if ((key === 'height' || key === 'minHeight' || key === 'minWidth') && value >= 44 && value <= 70) return [key, Math.max(44, value * scale)];
            return [key, value * scale];
        })),
    ])) as T, [styles, scale]);
}
