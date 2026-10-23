/** Defines work areas, South African province options and area matching. */
export type WorkArea = {
    placeId: string;
    label: string;
    countryCode: 'ZA';
    kind: 'country' | 'region';
    names: string[];
};
export const southAfrica: WorkArea = {placeId:'country:ZA',label:'South Africa',countryCode:'ZA',kind:'country',names:['South Africa']};
/** Builds the field summary from selected work areas. */
export function locationSummary(areas: WorkArea[]) { return areas.map(
    /** Transforms each item into the required display or state value. */
     area => area.label).join('; '); }
/** Checks whether an opportunity matches the preferred work areas. */
export function matchesWorkAreas(area: string, region: string, preferences: WorkArea[], legacy = '') {
    /** Normalises location text for comparison. */
    const normalize =  (value:string) => value.normalize('NFD').replace(/[\u0300-\u036f]/g,'').toLowerCase().replace(/[^a-z0-9]+/g,' ').trim();
    const target = normalize(area + ' ' + region);
    if (!preferences.length) return !!legacy && target.includes(normalize(legacy));
    return preferences.some(
        /** Checks the item against the current selection or criteria. */
         preference => preference.kind === 'country' || target.includes(normalize(preference.names[0] || preference.label)));
}


/** Creates a work-area record for a South African province. */
export const provinces: WorkArea[] = ['Eastern Cape','Free State','Gauteng','KwaZulu-Natal','Limpopo','Mpumalanga','Northern Cape','North West','Western Cape'].map( name=>({
    placeId:'province:ZA:'+name,countryCode:'ZA',kind:'region',label:name,names:[name,'South Africa'],
}));
