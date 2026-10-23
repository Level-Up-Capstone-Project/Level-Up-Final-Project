/** Displays an icon for the selected document file type. */
import { Image } from 'react-native';
/** Displays an icon for the selected document file type. */
export default function FileTypeIcon({ filename, size = 49 }: { filename: string; size?: number }) {
    const extension = filename.split('.').pop()?.toLowerCase();
    const source = extension === 'pdf' ? {uri:'/assets/upload/c985c.png'} : {uri:'/assets/upload/94f58.png'};
    return <Image accessible={false} source={source} resizeMode="contain" style={{ width: size, height: size }}/>;
}
