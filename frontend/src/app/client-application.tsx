"use client";
/** Loads the interactive screens after browser APIs are available. */
import dynamic from 'next/dynamic';
const Application=dynamic(()=>import('../App'),{ssr:false});
/** Displays the application without server-rendering browser-dependent components. */
export default function ClientApplication(){return <Application/>;}
