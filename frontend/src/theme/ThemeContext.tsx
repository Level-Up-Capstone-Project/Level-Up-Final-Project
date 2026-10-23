/** Supplies the application dark-theme setting to child components. */
import { createContext, useContext, type ReactNode } from 'react';

// The app uses its original dark design; the appearance toggle has been removed.
const darkTheme = { dark: true };
const ThemeContext = createContext(darkTheme);
/** Provides the application theme setting to its child components. */
export function ThemeProvider({ children }: { children: ReactNode }) {
    return <ThemeContext.Provider value={darkTheme}>{children}</ThemeContext.Provider>;
}
/** Returns the current application theme context. */
export const useTheme =  () => useContext(ThemeContext);
