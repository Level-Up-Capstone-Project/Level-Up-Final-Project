/** Introduces the assessment feature during onboarding. */
import IntroLayout, { type IntroProps } from '../onboarding/IntroLayout';
/** Introduces the assessment feature during onboarding. */
export default function IntroAssessmentScreen({ go }: IntroProps) {
    return <IntroLayout go={go} step={0} image={{uri:'/assets/onboarding/06f7a.png'}} title="ASSESS YOUR POSITION" description={'Build your profile, upload your CV and\nacademic record, and establish your\ncurrent career readiness.'} copyLeft={.1463}/>;
}
