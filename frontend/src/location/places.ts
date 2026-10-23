/** Connects area search and place selection to the configured search service. */
import type { WorkArea } from './types';
export type AreaSuggestion = { placeId:string; label:string };
const endpoint = process.env.NEXT_PUBLIC_PLACES_API_URL?.replace(/\/$/,'');
export const placesConfigured = !!endpoint;
/** Creates a session identifier for related area-search requests. */
export function createSessionToken() { return Date.now().toString(36) + Math.random().toString(36).slice(2) + Math.random().toString(36).slice(2); }
/** Sends a request to the configured area-search endpoint. */
async function request(path:string, body:object, signal?:AbortSignal) {
    if (!endpoint) throw new Error('Area search is not connected yet. You can select South Africa for now.');
    const response = await fetch(endpoint+path,{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify(body),signal});
    const data = await response.json();
    if (!response.ok) throw new Error(data.error || 'Area search is unavailable. Please try again.');
    return data;
}
/** Fetches area suggestions for the search input. */
export async function searchAreas(input:string, sessionToken:string, signal:AbortSignal):Promise<AreaSuggestion[]> {
    const data = await request('/autocomplete',{input,sessionToken},signal);
    return Array.isArray(data.suggestions) ? data.suggestions.filter(
        /** Selects the place that match the current conditions. */
         (item:AreaSuggestion)=>typeof item.placeId==='string' && typeof item.label==='string') : [];
}
/** Resolves a selected place and checks that it is within South Africa. */
export async function selectArea(placeId:string, sessionToken:string):Promise<WorkArea> {
    const data = await request('/details',{placeId,sessionToken});
    if (data.area?.countryCode !== 'ZA' || !data.area.placeId || !Array.isArray(data.area.names)) throw new Error('Choose an area within South Africa.');
    return data.area;
}
