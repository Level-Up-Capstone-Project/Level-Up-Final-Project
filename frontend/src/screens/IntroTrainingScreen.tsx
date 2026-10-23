/** Introduces learning pathways during onboarding. */
import IntroLayout, { type IntroProps } from '../onboarding/IntroLayout';
/** Introduces learning pathways during onboarding. */
export default function IntroTrainingScreen({ go }: IntroProps) {
    return <IntroLayout go={go} step={2} image={{uri:'/assets/onboarding/1e738.png'}} title="BUILD YOUR LOADOUT" description={'Strengthen key skills through personalised\nlearning paths, assessments and real-world\ndevelopment.'} copyLeft={.1156} titleOffset={10}/>;
}
