/** Introduces opportunity discovery during onboarding. */
import IntroLayout, { type IntroProps } from '../onboarding/IntroLayout';
/** Introduces opportunity discovery during onboarding. */
export default function IntroOpportunitiesScreen({ go }: IntroProps) {
    return <IntroLayout go={go} step={3} image={{uri:'/assets/onboarding/e03e6.png'}} title="UNLOCK YOUR NEXT MOVE" description={'Discover internships, graduate programmes\nand career opportunities matched to your\nreadiness and goals.'} copyLeft={.0961}/>;
}
