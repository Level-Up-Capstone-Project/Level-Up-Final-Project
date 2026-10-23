/** Displays the bundled target symbol at the requested size. */
import { Image } from 'react-native';

// Exact shared Figma artwork for target nodes 207:4902 and 207:4904.
/** Displays the bundled target symbol at the requested size. */
export default function TargetIcon({size=45,testID='figma-target-icon'}:{size?:number;testID?:string}) {
    return <Image testID={testID} accessible={false} source={{uri:'/assets/cv-report/target.png'}} resizeMode="contain" style={{width:size,height:size}}/>;
}
