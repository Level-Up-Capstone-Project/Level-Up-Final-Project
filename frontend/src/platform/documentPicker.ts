/** Provides browser document selection using the system file chooser. */
export type DocumentPickerAsset = {uri:string;name:string;mimeType?:string;size?:number;file?:File};
const urls=new Set<string>();
/** Opens a file picker and returns the selected file with a browser object URL. */
export function getDocumentAsync({type}:{type:string[];copyToCacheDirectory?:boolean}):Promise<{canceled:true}|{canceled:false;assets:DocumentPickerAsset[]}> {
 return new Promise(resolve=>{
  const input=document.createElement('input');input.type='file';input.accept=type.join(',');input.style.display='none';document.body.append(input);
  let settled=false;
  /** Removes the temporary file input and resolves its result once. */
  const finish=(result:{canceled:true}|{canceled:false;assets:DocumentPickerAsset[]})=>{if(settled)return;settled=true;input.remove();resolve(result);};
  /** Converts the chosen browser file into the document used by the upload screens. */
  input.addEventListener('change',()=>{const file=input.files?.[0];if(!file){finish({canceled:true});return;}const uri=URL.createObjectURL(file);urls.add(uri);finish({canceled:false,assets:[{uri,name:file.name,mimeType:file.type,size:file.size,file}]});},{once:true});
  /** Returns to the form when the chooser is dismissed without a selection. */
  input.addEventListener('cancel',()=>finish({canceled:true}),{once:true});input.click();
 });
}
/** Releases a document preview URL when its file is removed or replaced. */
export function releaseDocument(uri:string){if(urls.delete(uri))URL.revokeObjectURL(uri);}
