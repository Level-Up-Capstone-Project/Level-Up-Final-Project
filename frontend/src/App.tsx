/** Loads fonts and assembles the application screens, navigation, feedback and legal views. */
import LoginSuccessScreen from './screens/LoginSuccessScreen';
import SignedOutScreen from './screens/SignedOutScreen';
import UploadSuccessScreen from './screens/UploadSuccessScreen';
import PasswordResetSuccessScreen from './screens/PasswordResetSuccessScreen';
import AnalysisCompleteScreen from './screens/AnalysisCompleteScreen';
import ConsentWithdrawnScreen from './screens/ConsentWithdrawnScreen';
import CvAnalysisReportScreen from './screens/CvAnalysisReportScreen';
import AchievementsScreen from './screens/AchievementsScreen';
import { ThemeProvider } from './theme/ThemeContext';
import { SafeAreaProvider } from 'react-native-safe-area-context';
import SplashScreen from './screens/SplashScreen';
import IntroAssessmentScreen from './screens/IntroAssessmentScreen';
import IntroGapScreen from './screens/IntroGapScreen';
import IntroTrainingScreen from './screens/IntroTrainingScreen';
import IntroOpportunitiesScreen from './screens/IntroOpportunitiesScreen';
import AuthFlow from './auth/AuthFlow';
import LegalModal from './legal/LegalModal';
import type { AuthScreen } from './auth/account';
import useAppModel from './navigation/useAppModel';
import LoaderScreen from './screens/LoaderScreen';
import UploadScreen from './screens/UploadScreen';
import UploadTranscriptScreen from './screens/UploadTranscriptScreen';
import HomeScreen from './screens/HomeScreen';
import MiraChatScreen from './screens/MiraChatScreen';
import AnalysisScreen from './screens/AnalysisScreen';
import SkillsScreen from './screens/SkillsScreen';
import JobsScreen from './screens/JobsScreen';
import JobDetailScreen from './screens/JobDetailScreen';
import LearningScreen from './screens/LearningScreen';
import TopicScreen from './screens/TopicScreen';
import CompleteScreen from './screens/CompleteScreen';
import SettingsScreen from './screens/SettingsScreen';
import ProfileSettingsScreen from './screens/ProfileSettingsScreen';
import MissionHubScreen from './screens/MissionHubScreen';
/** Selects the current screen and connects it to shared application state. */
function CareerApp() {
    const model = useAppModel();
    const { screen, account, setAccount, go, replace, auth, legalDocument, openLegal, closeLegal } = model;
    if(model.feedback){const views={login:LoginSuccessScreen,signedOut:SignedOutScreen,upload:UploadSuccessScreen,passwordReset:PasswordResetSuccessScreen,analysisComplete:AnalysisCompleteScreen,consentWithdrawn:ConsentWithdrawnScreen};const Feedback=views[model.feedback.kind];return <Feedback onContinue={model.continueFeedback} message={model.feedback.message} nextScreen={model.feedback.next}/>;}
    const introductionScreens = { splash: SplashScreen, introAssessment: IntroAssessmentScreen, introGap: IntroGapScreen, introTraining: IntroTrainingScreen, introOpportunities: IntroOpportunitiesScreen };
    if (screen in introductionScreens) {
        const Introduction = introductionScreens[screen as keyof typeof introductionScreens];
        return <Introduction go={go}/>;
    }
    if (auth)
        return <><AuthFlow screen={screen as AuthScreen} navigate={go} account={account} updateAccount={setAccount} openLegal={openLegal} onAuthenticated={
            /** Saves the policy agreement selected by the user. */
             (signup, email) => {if(signup)model.consent.grant(email);model.resetTo(signup ? 'loader' : 'home');model.showFeedback('login',signup ? 'loader' : 'home');}} onPasswordReset={
            /** Handles the on password reset for this component. */
             ()=>model.showFeedback('passwordReset','signin')}/><LegalModal document={legalDocument} onClose={closeLegal}/></>;
    if (screen === 'loader') return <LoaderScreen onComplete={
        /** Opens the upload screen. */
         () => replace('upload')}/>;
    if (screen === 'upload') return <UploadScreen model={model}/>;
    if (screen === 'uploadTranscript') return <UploadTranscriptScreen model={model}/>;
    if (screen === 'home') return <HomeScreen model={model}/>;
    if (screen === 'missionHub') return <MissionHubScreen model={model}/>;
    if (screen === 'jobs') return <JobsScreen model={model}/>;
    if (screen === 'job') return <JobDetailScreen model={model}/>;
    if (screen === 'cvReport') return <CvAnalysisReportScreen model={model}/>;
    if (screen === 'skills') return <SkillsScreen model={model}/>;
    if (screen === 'analysis') return <AnalysisScreen model={model}/>;
    if (screen === 'topic') return <TopicScreen model={model}/>;
    if (screen === 'learning') return <LearningScreen model={model}/>;
    if (screen === 'complete') return <CompleteScreen model={model}/>;
    if (screen === 'achievements') return <AchievementsScreen model={model}/>;
    if (screen === 'profileSettings') return <ProfileSettingsScreen model={model}/>;
    if (screen === 'settings') return <><SettingsScreen model={model}/><LegalModal document={legalDocument} onClose={closeLegal}/></>;
    if (screen === 'mira') return <MiraChatScreen model={model}/>;
    return null;
}
/** Loads fonts and assembles the application screens, navigation, feedback and legal views. */
export default function App() {
    return <SafeAreaProvider><ThemeProvider><CareerApp /></ThemeProvider></SafeAreaProvider>;
}
