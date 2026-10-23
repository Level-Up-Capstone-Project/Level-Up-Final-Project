/** Defines the root document, application metadata and global styles. */
import type {Metadata,Viewport} from 'next';
import './globals.css';
export const metadata:Metadata={title:'Level Up',description:'Career development, skills and opportunities.',icons:{icon:'/icon.svg'}};
export const viewport:Viewport={width:'device-width',initialScale:1,themeColor:'#000'};
/** Wraps every page in the application document. */
export default function RootLayout({children}:{children:React.ReactNode}){return <html lang="en"><body>{children}</body></html>;}
