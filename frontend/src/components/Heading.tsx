/** Displays a heading using the shared application typography. */
import React from 'react';
import { useAppStyles } from './styles';
import Text from './Text';
/** Displays a heading using the shared application typography. */
export default function Heading({ children }: {
    children: React.ReactNode;
}) {
    const s = useAppStyles(); return <Text style={s.heading}>{children}</Text>; }
