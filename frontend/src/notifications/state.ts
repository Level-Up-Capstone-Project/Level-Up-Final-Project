/** Defines notification records and derives alerts from opportunity updates. */
import type { Opportunity } from '../opportunities/data';
export type OpportunityAlert = {id:string;opportunityId:string;title:string;company:string;match:number;kind:'new'|'improved';receivedAt:number;read:boolean};
export type NotificationState = {seen:Record<string,number>;alerts:OpportunityAlert[]};
/** Creates an empty notification record. */
export const emptyNotifications =  ():NotificationState => ({seen:{},alerts:[]});
/** Converts stored data into notification state. */
export function parseNotifications(raw:string|null):NotificationState {
    try {
        if(!raw)return emptyNotifications();
        const value=JSON.parse(raw);
        return {seen:Object.fromEntries(Object.entries(value.seen||{}).filter(
            /** Selects the item that match the current conditions. */
             ([,score])=>typeof score==='number'&&Number.isFinite(score)).map(
            /** Transforms each item into the required display or state value. */
             ([id,score])=>[id,score as number])),alerts:Array.isArray(value.alerts)?value.alerts.filter(
            /** Selects the alerts that match the current conditions. */
             (a:OpportunityAlert)=>typeof a?.id==='string'&&typeof a.opportunityId==='string'&&typeof a.title==='string'&&typeof a.company==='string'&&typeof a.receivedAt==='number'&&Number.isFinite(a.receivedAt)&&typeof a.match==='number'&&typeof a.read==='boolean'&&(a.kind==='new'||a.kind==='improved')).slice(0,100):[]};
    }catch{return emptyNotifications();}
}
/** Creates or updates alerts from newly received opportunity data. */
export function receiveOpportunities(state:NotificationState,opportunities:Opportunity[],receivedAt:number):NotificationState {
    let next=state;
    const ids=new Set(state.alerts.map(
        /** Transforms each alert into the required display or state value. */
         alert=>alert.id));
    for(const opportunity of opportunities){
        if (opportunity.match === null) continue;
        const match=Number.isFinite(opportunity.match)?Math.max(0,Math.min(100,opportunity.match)):0;
        const previous=next.seen[opportunity.id];
        if(previous===undefined||match>previous){
            if(next===state)next={seen:{...state.seen},alerts:[...state.alerts]};
            next.seen[opportunity.id]=match;
            const kind=previous===undefined?'new':'improved';
            const id=`${opportunity.id}:${kind}:${match}`;
            if(!ids.has(id)){
                ids.add(id);
                next.alerts.unshift({id,opportunityId:opportunity.id,title:opportunity.title,company:opportunity.company,match,kind,receivedAt,read:false});
            }
        }
    }
    if(next!==state)next.alerts=next.alerts.slice(0,100);
    return next;
}
