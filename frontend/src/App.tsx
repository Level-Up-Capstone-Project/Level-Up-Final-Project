/** Loads fonts and assembles the application screens, navigation, feedback and legal views. */
import LoginSuccessScreen from './screens/LoginSuccessScreen';
import SignedOutScreen from './screens/SignedOutScreen';
import UploadSuccessScreen from './screens/UploadSuccessScreen';
import PasswordResetSuccessScreen from './screens/PasswordResetSuccessScreen';
import AnalysisCompleteScreen from './screens/AnalysisCompleteScreen';
import ConsentWithdrawnScreen from './screens/ConsentWithdrawnScreen';
import CvAnalysisReportScreen from './screens/CvAnalysisReportScreen';
import AchievementsScreen from './screens/AchievementsScreen';
import BottomNavigation from './navigation/BottomNavigation';
import { View } from 'react-native';
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
import type { ReactNode } from 'react';
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
    const signedInLayout = (content: ReactNode) => !account.token ? content : <View style={{flex:1,backgroundColor:'#000'}}><View style={{flex:1,minHeight:0}}>{content}</View><View style={{width:'100%',maxWidth:480,alignSelf:'center'}}><BottomNavigation screen={screen} go={go} inline/></View></View>;
    if(model.feedback){const views={login:LoginSuccessScreen,signedOut:SignedOutScreen,upload:UploadSuccessScreen,passwordReset:PasswordResetSuccessScreen,analysisComplete:AnalysisCompleteScreen,consentWithdrawn:ConsentWithdrawnScreen};const Feedback=views[model.feedback.kind];return signedInLayout(<Feedback onContinue={model.continueFeedback} message={model.feedback.message} nextScreen={model.feedback.next}/>);}
    const introductionScreens = { splash: SplashScreen, introAssessment: IntroAssessmentScreen, introGap: IntroGapScreen, introTraining: IntroTrainingScreen, introOpportunities: IntroOpportunitiesScreen };
    if (screen in introductionScreens) {
        const Introduction = introductionScreens[screen as keyof typeof introductionScreens];
        return signedInLayout(<Introduction go={go}/>);
    }
    if (auth)
        return <><AuthFlow screen={screen as AuthScreen} navigate={go} account={account} updateAccount={setAccount} openLegal={openLegal} onAuthenticated={
            /** Saves the policy agreement selected by the user. */
             (signup, email) => {if(signup)model.consent.grant(email);model.resetTo(signup ? 'loader' : 'home');model.showFeedback('login',signup ? 'loader' : 'home');}} onPasswordReset={
            /** Handles the on password reset for this component. */
             ()=>model.showFeedback('passwordReset','signin')}/><LegalModal document={legalDocument} onClose={closeLegal}/></>;
    if (screen === 'loader') return signedInLayout(<LoaderScreen onComplete={
        /** Opens the upload screen. */
         () => replace('upload')}/>);
    if (screen === 'upload') return signedInLayout(<UploadScreen model={model}/>);
    if (screen === 'uploadTranscript') return signedInLayout(<UploadTranscriptScreen model={model}/>);
    if (screen === 'home') return signedInLayout(<HomeScreen model={model}/>);
    if (screen === 'missionHub') return signedInLayout(<MissionHubScreen model={model}/>);
    if (screen === 'jobs') return signedInLayout(<JobsScreen model={model}/>);
    if (screen === 'job') return signedInLayout(<JobDetailScreen model={model}/>);
    if (screen === 'cvReport') return signedInLayout(<CvAnalysisReportScreen model={model}/>);
    if (screen === 'skills') return signedInLayout(<SkillsScreen model={model}/>);
    if (screen === 'analysis') return signedInLayout(<AnalysisScreen model={model}/>);
    if (screen === 'topic') return signedInLayout(<TopicScreen model={model}/>);
    if (screen === 'learning') return signedInLayout(<LearningScreen model={model}/>);
    if (screen === 'complete') return signedInLayout(<CompleteScreen model={model}/>);
    if (screen === 'achievements') return signedInLayout(<AchievementsScreen model={model}/>);
    if (screen === 'profileSettings') return signedInLayout(<ProfileSettingsScreen model={model}/>);
    if (screen === 'settings') return signedInLayout(<><SettingsScreen model={model}/><LegalModal document={legalDocument} onClose={closeLegal}/></>);
    if (screen === 'mira') return signedInLayout(<MiraChatScreen model={model}/>);
    return null;
}
/** Loads fonts and assembles the application screens, navigation, feedback and legal views. */
export default function App() {
    return <SafeAreaProvider><ThemeProvider><CareerApp /></ThemeProvider></SafeAreaProvider>;
}
