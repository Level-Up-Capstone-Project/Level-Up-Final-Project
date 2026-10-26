/** Loads and mutates authenticated roadmap, resource progress and saved resources. */
import {useCallback, useEffect, useMemo, useState, type Dispatch, type SetStateAction} from 'react';
import {apiRequest, ApiError} from '../api/client';

type RoadmapItem = {roadmapItemID:string;skillID:string;title:string;description?:string;stepOrder:number;status:string};
type Roadmap = {roadmapID:string;careerID:string;careerName?:string;title:string;progressPercentage:number;items:RoadmapItem[]};
type CareerSuggestion = {careerID:string;careerName:string;matchedSkills:{skillName:string;fromCv:boolean;fromTranscript:boolean}[]};
type Resource = {resourceID:string;resourceName:string;resourceType:string;url:string;provider:string;difficultyLevel:string;costType:string;estDuration:string;description:string};
type ItemResource = {roadmapItemResourceID:string;isCompleted:boolean;completedAt:string|null;resource:Resource};
type Topic = RoadmapItem & {resources:ItemResource[]};
type State = {owner:string;roadmap:Roadmap|null;topics:Topic[];savedIDs:Set<string>;careerSuggestions:CareerSuggestion[];generatingCareerID:string|null;loading:boolean;error:string};

const empty = (owner:string):State => ({owner,roadmap:null,topics:[],savedIDs:new Set(),careerSuggestions:[],generatingCareerID:null,loading:false,error:''});

export default function useLearningProgress(account: string, token?: string, targetCareerID?: string) {
    const owner = account.toLowerCase();
    const [state,setState] = useState<State>(() => empty(owner));
    const [topic,setTopic] = useState('');
    const [savingProgress,setSavingProgress] = useState(false);

    const load = useCallback(async (signal?: AbortSignal) => {
        if (!token) {setState(empty(owner)); return;}
        setState(current => ({...(current.owner === owner ? current : empty(owner)),loading:true,error:''}));
        try {
            let roadmap:Roadmap;
            try {roadmap = await apiRequest<Roadmap>('/api/roadmaps/current',{token,signal});}
            catch (error) {
                if (!(error instanceof ApiError && error.status === 404)) throw error;
                if (targetCareerID) {
                    roadmap = await apiRequest<Roadmap>('/api/roadmaps/generate',{method:'POST',token,signal,body:{careerID:targetCareerID}});
                } else {
                    const result = await apiRequest<{suggestions:CareerSuggestion[]}>('/api/careers/suggestions',{token,signal});
                    if (signal?.aborted) return;
                    setState({...empty(owner),careerSuggestions:result.suggestions,loading:false});
                    setTopic('');
                    return;
                }
            }
            const [resourceLists,saved] = await Promise.all([
                Promise.all(roadmap.items.map(item => apiRequest<ItemResource[]>(`/api/roadmaps/items/${item.roadmapItemID}/resources`,{token,signal}))),
                apiRequest<{resourceID:string}[]>('/api/saved-resources',{token,signal}),
            ]);
            if (signal?.aborted) return;
            const topics = roadmap.items.map((item,index)=>({...item,resources:resourceLists[index]}));
            setState({...empty(owner),roadmap,topics,savedIDs:new Set(saved.map(item=>item.resourceID)),loading:false});
            setTopic(current => topics.some(item=>item.title===current) ? current : topics[0]?.title ?? '');
        } catch (error) {
            if (!signal?.aborted) setState(current=>({...current,owner,loading:false,error:error instanceof Error?error.message:'Could not load the learning roadmap.'}));
        }
    },[owner,token,targetCareerID]);

    useEffect(()=>{
        const controller=new AbortController();
        void load(controller.signal);
        return ()=>controller.abort();
    },[load]);

    const generateRoadmap=useCallback(async(careerID:string)=>{
        if(!token)return;
        setState(current=>({...current,loading:true,error:'',generatingCareerID:careerID}));
        try{
            await apiRequest<Roadmap>('/api/roadmaps/generate',{method:'POST',token,body:{careerID}});
            await load();
        }catch(error){
            setState(current=>({...current,loading:false,error:error instanceof Error?error.message:'Could not generate the learning roadmap.'}));
        }finally{
            setState(current=>({...current,generatingCareerID:null}));
        }
    },[load,token]);

    const data=state.owner===owner?state:empty(owner);
    const current=data.topics.find(item=>item.title===topic);
    const resourcesForTopic=useCallback((name:string)=>data.topics.find(item=>item.title===name)?.resources.map(item=>item.resource.resourceName)??[],[data.topics]);
    const learningGroups=useMemo(()=>data.topics.map(item=>({title:item.title,topics:[item.title]})),[data.topics]);
    const taskProgress=useMemo(()=>Object.fromEntries(data.topics.map(item=>[item.title,item.resources.map(resource=>resource.isCompleted)])),[data.topics]);
    const completed=useMemo(()=>data.topics.filter(item=>item.status==='completed'||(item.resources.length>0&&item.resources.every(resource=>resource.isCompleted))).map(item=>item.title),[data.topics]);
    const savedTopics=useMemo(()=>data.topics.filter(item=>item.resources.length>0&&item.resources.every(resource=>data.savedIDs.has(resource.resource.resourceID))).map(item=>item.title),[data.topics,data.savedIDs]);

    const completeResources=useCallback(async (selected:Topic, indexes:number[])=>{
        if(!token)return false;
        const pending=indexes.filter(index=>selected.resources[index]&&!selected.resources[index].isCompleted);
        if(!pending.length)return selected.status === 'completed';
        setSavingProgress(true);
        const pendingIndexes=new Set(pending);
        setState(currentState=>({...currentState,topics:currentState.topics.map(item=>item.roadmapItemID!==selected.roadmapItemID?item:{...item,resources:item.resources.map((resource,index)=>pendingIndexes.has(index)?{...resource,isCompleted:true,completedAt:new Date().toISOString()}:resource)})}));
        try {
            // Completing resources in sequence lets the server observe the final item
            // completion after all earlier resource updates have committed.
            for (const index of pending) {
                const resource=selected.resources[index].resource;
                await apiRequest(`/api/roadmaps/items/${selected.roadmapItemID}/resources/${resource.resourceID}/complete`,{method:'POST',token});
            }
            await load();
            return true;
        } catch(error){await load();setState(currentState=>({...currentState,error:error instanceof Error?error.message:'Could not save learning progress.'}));return false;}
        finally {setSavingProgress(false);}
    },[token,load]);

    const setChecked:Dispatch<SetStateAction<boolean[]>>=action=>{
        if(!current)return;
        const previous=current.resources.map(resource=>resource.isCompleted);
        const next=typeof action==='function'?action(previous):action;
        void completeResources(current,next.map((value,index)=>value&&!previous[index]?index:-1).filter(index=>index>=0));
    };
    const completeTopic=useCallback(async(name:string)=>{
        if (savingProgress) return false;
        const selected=data.topics.find(item=>item.title===name);
        if(!selected)return false;
        return completeResources(selected,selected.resources.map((_,index)=>index));
    },[completeResources,data.topics,savingProgress]);
    const finishCurrentMission=async()=>{
        if(!current || !current.resources.length || savingProgress)return false;
        if(current.status === 'completed')return true;
        return completeResources(current,current.resources.map((_,index)=>index));
    };
    const toggleSavedTopic=async()=>{
        if(!token||!current||!current.resources.length)return;
        const remove=savedTopics.includes(current.title);
        const resources=current.resources.filter(item=>remove
            ? data.savedIDs.has(item.resource.resourceID)
            : !data.savedIDs.has(item.resource.resourceID));
        try{
            await Promise.all(resources.map(item=>apiRequest(`/api/saved-resources/${item.resource.resourceID}`,{method:remove?'DELETE':'POST',token})));
            setState(value=>{const savedIDs=new Set(value.savedIDs);resources.forEach(item=>remove?savedIDs.delete(item.resource.resourceID):savedIDs.add(item.resource.resourceID));return {...value,savedIDs};});
        }catch(error){setState(value=>({...value,error:error instanceof Error?error.message:'Could not update saved resources.'}));}
    };

    return {savedTopics,toggleSavedTopic,topic,setTopic,currentTopic:current,checked:current?.resources.map(resource=>resource.isCompleted)??[],setChecked,
        completed,completeTopic,taskProgress,learningGroups,resourcesForTopic,roadmapCareerName:data.roadmap?.careerName??'',
        careerSuggestions:data.careerSuggestions,generatingCareerID:data.generatingCareerID,generateRoadmap,
        learningLoading:data.loading,learningError:data.error,reloadLearning:load,finishCurrentMission,savingProgress};
}
