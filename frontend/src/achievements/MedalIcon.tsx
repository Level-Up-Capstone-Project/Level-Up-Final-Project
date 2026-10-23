/** Draws the available small gold medal symbols. */
import Svg, {Circle,Ellipse,Path,Rect} from 'react-native-svg';
/** Draws the available small gold medal symbols. */
export default function MedalIcon({name,size=24}:{name:string;size?:number}) {
const paths:Record<string,string>={mountain:'M2 21L8 3L12 12L17 6L22 21ZM6 14L10 17L14 14L19 17',book:'M12 5C8 2 4 3 3 4V19C6 17 9 18 12 21C15 18 18 17 21 19V4C18 2 15 3 12 5ZM12 5V21',compass:'M16 8L14 14L8 16L10 10Z',briefcase:'M9 7V3H15V7M3 12C9 16 15 16 21 12M11 12H13',route:'M7 19L17 12M7 5L17 12',radar:'M12 12L19 5M4 7L2 5M20 17L22 19M3 15L1 16M21 9L23 8',flag:'M5 22V3C10 0 14 6 20 3V14C14 17 10 11 5 14',target:'M12 1V6M12 18V23M1 12H6M18 12H23',heart:'M12 21C-7 9 4-3 12 5C20-3 31 9 12 21Z',building:'M3 8L12 2L21 8ZM5 10V19M10 10V19M15 10V19M20 10V19M3 19H21V22H3Z',star:'M12 2L15 8L22 9L17 14L18 21L12 18L6 21L7 14L2 9L9 8Z',globe:'M2 12H22M12 2C5 8 5 16 12 22C19 16 19 8 12 2ZM12 2V22',award:'M8 15L6 23L12 20L18 23L16 15'};
return <Svg width={size} height={size} viewBox="0 0 24 24" fill="none" stroke="#f2b544" strokeWidth={1.8} strokeLinecap="round" strokeLinejoin="round">
{['compass','radar','target','globe'].includes(name)&&<Circle cx={12} cy={12} r={9}/>}
{name==='radar'&&<><Circle cx={12} cy={12} r={5}/><Circle cx={12} cy={12} r={1}/></>}
{name==='award'&&<Circle cx={12} cy={9} r={6}/>}
{name==='briefcase'&&<Rect x={3} y={7} width={18} height={14} rx={2}/>}
{name==='bars'&&<><Rect x={3} y={11} width={3} height={10} rx={1} fill="#f2b544"/><Rect x={10} y={4} width={3} height={17} rx={1} fill="#f2b544"/><Rect x={17} y={8} width={3} height={13} rx={1} fill="#f2b544"/></>}
{name==='route'&&<><Circle cx={7} cy={5} r={3}/><Circle cx={17} cy={12} r={3}/><Circle cx={7} cy={19} r={3}/></>}
{name==='paw'&&<><Ellipse cx={11} cy={16} rx={5} ry={7} transform="rotate(-30 11 16)"/><Circle cx={5} cy={5} r={2}/><Circle cx={13} cy={3} r={2}/><Circle cx={20} cy={8} r={2}/></>}
{paths[name]&&<Path d={paths[name]}/>}
</Svg>;
}
