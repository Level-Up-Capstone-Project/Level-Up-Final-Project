/** Introduces skill-gap discovery during onboarding. */
import IntroLayout, { type IntroProps } from '../onboarding/IntroLayout';
/** Introduces skill-gap discovery during onboarding. */
export default function IntroGapScreen({ go }: IntroProps) {
    return <IntroLayout go={go} step={1} image={{uri:'/assets/onboarding/427a0.png'}} title="IDENTIFY THE GAP" description={'Select your target career and uncover the\nskills, experience and knowledge standing\nbetween you and the objective.'} copyLeft={.1156}/>;
}
