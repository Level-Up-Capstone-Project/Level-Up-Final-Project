/** Provides the backend proxy for South African area search and place details. */
const http=require('node:http');
const {readFileSync,existsSync}=require('node:fs');
const path=require('node:path');
const allowedTypes=new Set(['country','administrative_area_level_1','administrative_area_level_2','administrative_area_level_3','administrative_area_level_4','locality','sublocality','sublocality_level_1','sublocality_level_2','sublocality_level_3','sublocality_level_4','sublocality_level_5','postal_town','neighborhood']);
/** Creates the HTTP server for area autocomplete and place details. */
function createPlacesServer({apiKey=process.env.GOOGLE_MAPS_API_KEY,googleFetch=fetch,allowedOrigin=process.env.PLACES_ALLOWED_ORIGIN || 'http://localhost:3000'}={}){
 const requests=new Map();
 return http.createServer(
     /** Submits the selected prompt or the current message draft. */
      async(req,res)=>{
  res.setHeader('Cache-Control','no-store');
  if(req.headers.origin===allowedOrigin){res.setHeader('Access-Control-Allow-Origin',allowedOrigin);res.setHeader('Vary','Origin');}
  res.setHeader('Access-Control-Allow-Headers','Content-Type');res.setHeader('Access-Control-Allow-Methods','POST, OPTIONS');
  /** Writes a JSON response with its HTTP status. */
  const send= (status,data)=>{res.writeHead(status,{'Content-Type':'application/json'});res.end(JSON.stringify(data));};
  if(req.method==='OPTIONS'){res.writeHead(204);return res.end();}
  if(req.method!=='POST' || !['/autocomplete','/details'].includes(req.url))return send(404,{error:'Not found.'});
  const now=Date.now(),ip=req.socket.remoteAddress;
  for(const [key,value] of requests)if(value.until<now)requests.delete(key);
  const rate=requests.get(ip) || {count:0,until:now+60000};rate.count++;requests.set(ip,rate);
  if(rate.count>90)return send(429,{error:'Too many searches. Try again shortly.'});
  if(!apiKey)return send(503,{error:'Area search is not connected yet.'});
  try{
   let size=0,chunks=[];for await(const chunk of req){size+=chunk.length;if(size>4096)return send(413,{error:'Request too large.'});chunks.push(chunk);}
   let body;try{body=JSON.parse(Buffer.concat(chunks).toString());}catch{return send(400,{error:'Invalid request.'});}
   if(!/^[a-zA-Z0-9_-]{1,36}$/.test(body.sessionToken || ''))return send(400,{error:'Invalid search session.'});
   const headers={'Content-Type':'application/json','X-Goog-Api-Key':apiKey};
   let response;
   if(req.url==='/autocomplete'){
    if(typeof body.input!=='string' || body.input.trim().length<2 || body.input.length>150)return send(400,{error:'Enter between 2 and 150 characters.'});
    response=await googleFetch('https://places.googleapis.com/v1/places:autocomplete',{method:'POST',headers,signal:AbortSignal.timeout(10000),body:JSON.stringify({input:body.input.trim(),includedRegionCodes:['za'],includedPrimaryTypes:['(regions)'],languageCode:'en',regionCode:'za',sessionToken:body.sessionToken})});
    if(!response.ok)return send(502,{error:'Area search is unavailable. Please try again.'});
    const data=await response.json();
    return send(200,{suggestions:(data.suggestions || []).filter(
        /** Selects the place that match the current conditions. */
         item=>item.placePrediction).map(
        /** Transforms each place into the required display or state value. */
         ({placePrediction:p})=>({placeId:p.placeId,label:p.text.text}))});
   }
   if(typeof body.placeId!=='string' || !/^[a-zA-Z0-9_-]{1,256}$/.test(body.placeId))return send(400,{error:'Invalid area.'});
   response=await googleFetch('https://places.googleapis.com/v1/places/'+encodeURIComponent(body.placeId)+'?languageCode=en&sessionToken='+encodeURIComponent(body.sessionToken),{headers:{...headers,'X-Goog-FieldMask':'id,displayName,addressComponents,types'},signal:AbortSignal.timeout(10000)});
   if(!response.ok)return send(502,{error:'Unable to select this area. Try again.'});
   const place=await response.json(),components=place.addressComponents || [];
   /** Identifies the country in the resolved place details. */
   const country=components.find( item=>item.types.includes('country'));
   if(country?.shortText!=='ZA' || !place.types?.some(
       /** Checks the place against the current selection or criteria. */
        type=>allowedTypes.has(type)) || place.types.some(
       /** Checks the place against the current selection or criteria. */
        type=>['street_address','premise','subpremise','route','establishment'].includes(type)))return send(400,{error:'Choose a province, town, suburb or region within South Africa.'});
   const names=[place.displayName?.text,...components.filter(
       /** Selects the place that match the current conditions. */
        item=>item.types.some(
       /** Checks the place against the current selection or criteria. */
        type=>allowedTypes.has(type))).map(
       /** Transforms each place into the required display or state value. */
        item=>item.longText)].filter(Boolean);
   const kind=place.types.includes('country')?'country':'region';
   const province=components.find(
       /** Checks the place against the current selection or criteria. */
        item=>item.types.includes('administrative_area_level_1'))?.longText;
   const label=[place.displayName?.text,province,'South Africa'].filter(
       /** Selects the place that match the current conditions. */
        (name,index,all)=>name && all.indexOf(name)===index).join(', ');
   return send(200,{area:{placeId:place.id,label,countryCode:'ZA',kind,names:[...new Set(names)]}});
  }catch{return send(502,{error:'Area search could not be completed. Please try again.'});}
 });
}
module.exports={createPlacesServer};
if(require.main===module){
 const env=path.join(__dirname,'.env');
 if(existsSync(env))process.loadEnvFile(env);
 const port=Number(process.env.PLACES_PORT || 3001);
 createPlacesServer().listen(port,'0.0.0.0',
     /** Reports that the local area-search server is listening. */
      ()=>console.log('Places search server listening on port '+port+(process.env.GOOGLE_MAPS_API_KEY?'':' (Google API key not configured)')));
}
