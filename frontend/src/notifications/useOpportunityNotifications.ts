/** Manages account alerts and read status as opportunity data changes. */
import { useEffect, useState } from 'react';
import type { Opportunity } from '../opportunities/data';
import { receiveOpportunities, type NotificationState } from './state';
import { loadNotifications, saveNotifications } from './storage';
/** Manages account alerts and read status as opportunity data changes. */
export default function useOpportunityNotifications(account:string,opportunities:Opportunity[]) {
    /** Loads the initial notification state for the current account. */
    const [stored,setStored]=useState( ()=>({owner:account,data:loadNotifications(account)}));
    const data=stored.owner===account?stored.data:loadNotifications(account);
    useEffect(
        /** Loads and synchronises state for the selected account. */
         ()=>{setStored(
        /** Builds the next stored value from its previous state. */
         old=>{
        const previous=old.owner===account?old.data:loadNotifications(account);
        const next=receiveOpportunities(previous,opportunities,Date.now());
        return old.owner===account&&next===old.data?old:{owner:account,data:next};
    });},[account,opportunities]);
    useEffect(
        /** Saves the current account state when it changes. */
         ()=>{if(stored.owner===account)saveNotifications(account,stored.data);},[account,stored]);
    /** Applies a change to the current account notification state. */
    const update= (action:(state:NotificationState)=>NotificationState)=>setStored(
        /** Builds the next stored value from its previous state. */
         old=>({owner:account,data:action(old.owner===account?old.data:loadNotifications(account))}));
    /** Marks one notification as read. */
    const markRead= (id:string)=>update(
        /** Marks the selected alert as read in the notification record. */
         old=>({...old,alerts:old.alerts.map(
        /** Transforms each alert into the required display or state value. */
         alert=>alert.id===id?{...alert,read:true}:alert)}));
    /** Marks every current notification as read. */
    const markAllRead= ()=>update(
        /** Marks all alerts as read in the notification record. */
         old=>({...old,alerts:old.alerts.map(
        /** Transforms each alert into the required display or state value. */
         alert=>({...alert,read:true}))}));
    return {alerts:data.alerts,unread:data.alerts.filter(
        /** Selects the alerts that match the current conditions. */
         alert=>!alert.read).length,markRead,markAllRead};
}
