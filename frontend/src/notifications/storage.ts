/** Reads and writes account notification state in browser storage. */
import { emptyNotifications, parseNotifications, type NotificationState } from './state';
/** Reads saved notification state for an account. */
export function loadNotifications(account:string):NotificationState {try{return parseNotifications(localStorage.getItem(`career-path-alerts-${account.toLowerCase()}`));}catch{return emptyNotifications();}}
/** Writes notification state for an account. */
export function saveNotifications(account:string,state:NotificationState){try{localStorage.setItem(`career-path-alerts-${account.toLowerCase()}`,JSON.stringify(state));}catch{/* Retain session alerts if storage is unavailable. */}}
