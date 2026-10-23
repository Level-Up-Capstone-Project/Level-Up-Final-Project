import {useState} from 'react';
import {Image, Modal, Platform, Pressable, ScrollView, Share, StyleSheet, View} from 'react-native';
import Svg,{Circle} from 'react-native-svg';
import Text from '../components/Text';
import {apiRequest, ApiError} from '../api/client';

type RoadmapItem = {roadmapItemID:string; skillID:string; title:string};
type AssessmentQuestion = {questionID:string; questionText:string; choices:string[]; questionOrder:number};
type Assessment = {skillAssessmentID:string; attemptNumber:number; questions:AssessmentQuestion[]};
type Result = {scorePercentage:number; correctAnswers:number; totalQuestions:number; perfectScore:boolean; answers:{questionID:string;questionText:string;userAnswer:string;correctAnswer:string;isCorrect:boolean}[]; skill:{skillName:string; status:string; isKnowledgeVerified:boolean}};

export default function KnowledgeCheckAction({token, item, displayName, onDashboard}:{token?:string;item?:RoadmapItem;displayName?:string;onDashboard:()=>void}) {
    const [visible,setVisible] = useState(false);
    const [assessment,setAssessment] = useState<Assessment|null>(null);
    const [answers,setAnswers] = useState<Record<string,string>>({});
    const [result,setResult] = useState<Result|null>(null);
    const [busy,setBusy] = useState(false);
    const [error,setError] = useState('');
    const [reviewing,setReviewing] = useState(false);
    const [showingIntro,setShowingIntro] = useState(false);
    const [retrying,setRetrying] = useState(false);

    const start = async () => {
        if (!token || !item || busy) return;
        setShowingIntro(false); setRetrying(false); setBusy(true); setError(''); setResult(null); setReviewing(false); setAnswers({}); setAssessment(null);
        try {
            const generated = await apiRequest<Assessment>('/api/assessments', {
                method:'POST', token, body:{skillID:item.skillID,roadmapItemID:item.roadmapItemID},
            });
            if (!generated.questions.length) throw new Error('No questions were returned. Please try again.');
            setAssessment(generated);
        } catch (cause) {
            setError(cause instanceof ApiError ? cause.message : cause instanceof Error ? cause.message : 'Could not start the knowledge check.');
        } finally {setBusy(false);}
    };

    const openIntro = () => {
        setVisible(true);
        setShowingIntro(true);
        setRetrying(false);
        setError('');
        setResult(null);
        setAssessment(null);
        setAnswers({});
        setReviewing(false);
    };

    const submit = async () => {
        if (!token || !assessment || busy) return;
        setBusy(true); setError('');
        try {
            const submitted = await apiRequest<Result>(`/api/assessments/${assessment.skillAssessmentID}/submit`, {
                method:'POST', token, body:{responses:assessment.questions.map(question=>({
                    questionID:question.questionID,userAnswer:answers[question.questionID],
                }))},
            });
            setResult(submitted);
        } catch (cause) {
            setError(cause instanceof ApiError ? cause.message : 'Could not submit your answers. Please try again.');
        } finally {setBusy(false);}
    };

    const shareResult = async () => {
        if (!result) return;
        const message=`I scored ${result.correctAnswers}/${result.totalQuestions} (${Math.round(result.scorePercentage)}%) on my ${result.skill.skillName} knowledge check.`;
        setError('');
        try {
            if (Platform.OS==='web' && typeof navigator!=='undefined' && typeof navigator.share!=='function') {
                if (!navigator.clipboard) throw new Error('Sharing is not supported in this browser.');
                await navigator.clipboard.writeText(message);
                setError('Result copied to clipboard.');
                return;
            }
            await Share.share({
                title:'Assessment results',
                message,
            });
        } catch (cause) {
            if (cause instanceof Error && cause.name==='AbortError') return;
            setError(cause instanceof Error ? cause.message : 'Could not share your assessment result.');
        }
    };

    const allAnswered = !!assessment && assessment.questions.every(question=>answers[question.questionID]);
    const firstMissedAnswer = result?.answers.find(answer=>!answer.isCorrect);
    const firstMissedQuestion = firstMissedAnswer
        ? assessment?.questions.find(question=>question.questionID===firstMissedAnswer.questionID)?.questionOrder
            ?? (result?.answers.findIndex(answer=>!answer.isCorrect) ?? -1) + 1
        : -1;
    return <>
        <Pressable testID="knowledge-check-start" accessibilityRole="button" accessibilityLabel="Start Knowledge Check"
            accessibilityState={{disabled:!token || !item}} disabled={!token || !item} onPress={openIntro} style={[s.action,(!token || !item)&&s.disabled]}>
            <Image testID="mission-knowledge-icon" source={{uri:'/assets/mission-complete/knowledge-check.png'}} style={s.icon} resizeMode="contain"/>
            <View style={s.copy}><Text style={s.title}>Knowledge Check</Text><Text style={s.subtitle}>{item?.title ?? 'Complete a roadmap mission to unlock this assessment.'}</Text></View>
            <Text style={s.arrow}>›</Text>
        </Pressable>
        <Modal visible={visible} transparent={!result&&!showingIntro} animationType="fade" onRequestClose={()=>setVisible(false)}>
            <View style={result||showingIntro||retrying?s.resultScrim:s.scrim}><View accessibilityViewIsModal style={[s.dialog,(result||showingIntro||retrying)&&s.fullscreenDialog,result&&s.resultDialog,showingIntro&&s.introDialog,retrying&&s.retryDialog]}>
                {retrying && result ? <>
                    <Image source={{uri:'/assets/mission-complete/landscape.png'}} resizeMode="cover" style={s.retryBackground}/>
                    <View pointerEvents="none" style={s.retryShade}/>
                    <View style={s.retryHeader}>
                        <Pressable accessibilityRole="button" accessibilityLabel="Back to assessment results"
                            onPress={()=>setRetrying(false)} style={s.back}>
                            <Text style={s.backText}>‹</Text>
                        </Pressable>
                        <Text accessibilityRole="header" style={s.retryHeaderTitle}>Try Again</Text>
                        <View style={s.back}/>
                    </View>
                    <View style={s.retryMessage}>
                        <View accessibilityLabel="Retry" style={s.retryIcon}>
                            <Svg width={52} height={52} viewBox="0 0 52 52">
                                <Circle cx={26} cy={26} r={23} fill="none" stroke="#d7b24d" strokeWidth={2}/>
                            </Svg>
                            <Text style={s.retryArrow}>↶</Text>
                        </View>
                        <Text style={s.retryTitle}>Every attempt helps you grow.</Text>
                        <Text style={s.retryCopy}>Not quite right this time. Learn the concept and give it another try.</Text>
                    </View>
                    <View style={s.retryProgress}>
                        <View>
                            <Text style={s.retryQuestion}>Question {firstMissedQuestion>=0?firstMissedQuestion:result.totalQuestions}</Text>
                            <Text style={s.retryNote}>Your learning progress is retained.</Text>
                        </View>
                        <Text style={s.retryAttempt}>Attempt {assessment?.attemptNumber ?? 1}</Text>
                    </View>
                    <View style={s.retryActions}>
                        <Pressable testID="assessment-try-again" accessibilityRole="button"
                            accessibilityState={{disabled:busy}} disabled={busy} onPress={()=>void start()}
                            style={[s.retryPrimary,busy&&s.disabled]}>
                            <Text style={s.retryPrimaryText}>{busy?'Preparing…':'Try Again  →'}</Text>
                        </Pressable>
                        <Pressable testID="assessment-review-explanation" accessibilityRole="button"
                            onPress={()=>{setRetrying(false);setReviewing(true);}} style={s.retrySecondary}>
                            <Text style={s.retrySecondaryText}>Review Explanation</Text>
                        </Pressable>
                    </View>
                </> : result ? <>
                    <Image source={{uri:'/assets/mission-complete/landscape.png'}} resizeMode="cover" style={s.resultBackground}/>
                    <View pointerEvents="none" style={s.resultShade}/>
                    <ScrollView style={s.resultContent} contentContainerStyle={s.resultContentContainer} keyboardShouldPersistTaps="handled">
                        <View style={s.resultHeader}>
                            <Pressable accessibilityRole="button" accessibilityLabel="Back to mission" onPress={()=>setVisible(false)} style={s.back}><Text style={s.backText}>‹</Text></Pressable>
                            <Text accessibilityRole="header" style={s.resultHeaderTitle}>{reviewing?'Review Answers':'Assessment Results'}</Text>
                            <Pressable testID="assessment-share" accessibilityRole="button" accessibilityLabel="Share assessment result"
                                onPress={()=>void shareResult()} style={s.back}><Text style={s.shareIcon}>⇧</Text></Pressable>
                        </View>
                    {!reviewing ? <>
                        {!!displayName&&<View style={s.nameBadge}><Text style={s.nameText}>{displayName}</Text></View>}
                        <View accessibilityLabel={`${result.correctAnswers} out of ${result.totalQuestions} correct`} style={s.scoreRing}>
                            <Svg width={92} height={92} viewBox="0 0 92 92">
                                <Circle cx={46} cy={46} r={39} fill="none" stroke="#41403b" strokeWidth={5}/>
                                <Circle cx={46} cy={46} r={39} fill="none" stroke="#d7b24d" strokeWidth={5}
                                    strokeDasharray={`${2*Math.PI*39*result.scorePercentage/100} ${2*Math.PI*39}`}
                                    transform="rotate(-90 46 46)"/>
                            </Svg>
                            <View style={s.scoreCenter}><Text testID="assessment-score" style={s.scoreValue}>{result.correctAnswers}/{result.totalQuestions}</Text>
                                <Text style={s.scoreCaption}>{Math.round(result.scorePercentage)}%</Text></View>
                        </View>
                        <Text style={s.resultTitle}>{result.perfectScore?'Outstanding!':'Great Progress!'}</Text>
                        <Text style={s.resultCopy}>You’re building momentum towards your career goals.</Text>
                        <View style={s.statusChip}><Text style={s.statusChipText}>{result.skill.isKnowledgeVerified?'KNOWLEDGE VERIFIED':'SKILL BUILDER'}</Text></View>
                        <View style={s.statRow}>
                            <View style={s.statCard}><Text style={[s.statIcon,s.correctIcon]}>✓</Text><Text style={s.statValue}>{result.correctAnswers}</Text><Text style={s.statLabel}>Correct</Text></View>
                            <View style={s.statCard}><Text style={[s.statIcon,s.reviewIcon]}>▤</Text><Text style={s.statValue}>{result.totalQuestions-result.correctAnswers}</Text><Text style={s.statLabel}>To review</Text></View>
                            <View style={s.statCard} accessibilityLabel="XP earned is not available"><Text style={[s.statIcon,s.skillIcon]}>ϟ</Text><Text style={s.statValue}>—</Text><Text style={s.statLabel}>XP earned</Text></View>
                        </View>
                        <View style={s.breakdownCard}>
                            <Text style={s.sectionTitle}>Your skill breakdown</Text>
                            <View style={s.skillRow}><Text style={s.skillName}>{result.skill.skillName}</Text><Text style={s.skillScore}>{Math.round(result.scorePercentage)}%</Text></View>
                            <View accessibilityRole="progressbar" accessibilityLabel={`${result.skill.skillName} score`} accessibilityValue={{min:0,max:100,now:result.scorePercentage}} style={s.track}>
                                <View style={[s.fill,{width:`${Math.max(0,Math.min(100,result.scorePercentage))}%`}]}/>
                            </View>
                        </View>
                        <View style={s.nextStep}>
                            <Text style={s.nextTitle}>◎  Your next step</Text>
                            <Text style={s.nextCopy}>{result.perfectScore
                                ? 'Knowledge verified. Keep practising to maintain your momentum.'
                                : `Review the questions you missed to strengthen your ${result.skill.skillName} skills.`}</Text>
                        </View>
                        {!!error&&<Text accessibilityRole="alert" style={s.error}>{error}</Text>}
                        {firstMissedQuestion>=0&&<Pressable testID="assessment-open-retry" accessibilityRole="button"
                            onPress={()=>setRetrying(true)} style={s.retryLink}>
                            <Text style={s.retryLinkText}>Practise the missed questions</Text>
                        </Pressable>}
                    </> : <View style={s.reviewList}>
                        {result.answers.map((answer,index)=><View key={answer.questionID} style={s.reviewCard}>
                            <Text style={s.reviewQuestion}>{index+1}. {answer.questionText}</Text>
                            <Text style={[s.reviewAnswer,answer.isCorrect?s.correctText:s.incorrectText]}>
                                {answer.isCorrect?'✓ Correct':'✕ Your answer'}: {answer.userAnswer}
                            </Text>
                            {!answer.isCorrect&&<Text style={s.reviewCorrect}>Correct answer: {answer.correctAnswer}</Text>}
                        </View>)}
                    </View>}
                    </ScrollView>
                    <View style={s.resultActions}>
                        {!reviewing
                            ? <Pressable testID="assessment-review-answers" accessibilityRole="button" onPress={()=>setReviewing(true)} style={s.resultPrimary}>
                                <Text style={s.resultPrimaryText}>Review Answers  →</Text>
                            </Pressable>
                            : <Pressable testID="assessment-back-to-results" accessibilityRole="button" onPress={()=>setReviewing(false)} style={s.resultPrimary}>
                                <Text style={s.resultPrimaryText}>Back to Results</Text>
                            </Pressable>}
                        <Pressable testID="knowledge-check-dashboard" accessibilityRole="button" onPress={()=>{setVisible(false);onDashboard();}} style={s.resultSecondary}>
                            <Text style={s.dashboardText}>Back to Dashboard</Text>
                        </Pressable>
                    </View>
                </> : showingIntro ? <>
                    <Image testID="knowledge-check-intro-background" source={{uri:'/assets/home/landscape.png'}}
                        resizeMode="cover" style={s.introBackground}/>
                    <View pointerEvents="none" style={s.introBackgroundShade}/>
                    <View style={s.introHero}>
                        <Pressable accessibilityRole="button" accessibilityLabel="Back to mission" onPress={()=>setVisible(false)} style={s.introBack}>
                            <Text style={s.backText}>‹</Text>
                        </Pressable>
                        <Text accessibilityRole="header" style={s.introHeroTitle}>TEST YOUR{'\n'}KNOWLEDGE</Text>
                        <Text style={s.introHeroCaption}>Discover your strengths. Identify gaps.{'\n'}Build skills for your future.</Text>
                    </View>
                    <ScrollView style={s.introScroll} contentContainerStyle={s.introScrollContent}>
                        <Text style={s.introEyebrow}>YOUR NEXT CHALLENGE</Text>
                        <Text style={s.introTitle}>Test Your Knowledge</Text>
                        <Text style={s.introCopy}>Discover your strengths. Identify gaps. Build skills for your future.</Text>
                        <View style={s.introStats}>
                            <View style={s.introStat}><Text style={s.introStatIcon}>☷</Text><Text style={s.introStatValue}>—</Text><Text style={s.introStatLabel}>Questions</Text></View>
                            <View style={s.introStat}><Text style={s.introStatIcon}>◷</Text><Text style={s.introStatValue}>Self-paced</Text><Text style={s.introStatLabel}>Duration</Text></View>
                            <View style={s.introStat}><Text style={s.introStatIcon}>ϟ</Text><Text style={s.introStatValue}>—</Text><Text style={s.introStatLabel}>XP available</Text></View>
                        </View>
                        <View style={s.skillsCard}>
                            <Text style={s.skillsTitle}>Skills you’ll explore</Text>
                            <View style={s.skillBullet}><Text style={s.skillBulletIcon}>◎</Text><Text style={s.skillBulletText}>{item?.title ?? 'Selected roadmap skill'}</Text></View>
                        </View>
                        {!!error&&<Text accessibilityRole="alert" style={s.error}>{error}</Text>}
                        <Pressable testID="knowledge-check-begin" accessibilityRole="button"
                            accessibilityState={{disabled:busy}} disabled={busy} onPress={()=>void start()}
                            style={[s.introPrimary,busy&&s.disabled]}>
                            <Text style={s.introPrimaryText}>{busy?'Preparing assessment…':'Start Assessment  →'}</Text>
                        </Pressable>
                        <Text style={s.introFootnote}>You can review your results when you finish.</Text>
                    </ScrollView>
                </> : <>
                <Text accessibilityRole="header" style={s.heading}>Knowledge Check</Text>
                {!!assessment && !result && <Text style={s.subtitle}>Attempt {assessment.attemptNumber} · {item?.title}</Text>}
                <ScrollView style={s.questions} keyboardShouldPersistTaps="handled">
                    {busy && !assessment && <Text style={s.subtitle}>Preparing questions…</Text>}
                    {assessment && !result && assessment.questions.map((question,index)=><View key={question.questionID} style={s.question}>
                        <Text style={s.questionText}>{index+1}. {question.questionText}</Text>
                        {question.choices.map(choice=><Pressable key={choice} accessibilityRole="radio"
                            accessibilityState={{selected:answers[question.questionID]===choice}}
                            onPress={()=>setAnswers(current=>({...current,[question.questionID]:choice}))}
                            style={[s.choice,answers[question.questionID]===choice&&s.choiceSelected]}>
                            <Text style={s.choiceText}>{choice}</Text>
                        </Pressable>)}
                    </View>)}
                </ScrollView>
                {!!error&&<Text accessibilityRole="alert" style={s.error}>{error}</Text>}
                <View style={s.actions}>
                    <Pressable accessibilityRole="button" onPress={()=>setVisible(false)} style={s.secondary}><Text style={s.actionText}>Close</Text></Pressable>
                    {!assessment&&!!error&&<Pressable accessibilityRole="button" disabled={busy} onPress={()=>void start()} style={s.primary}><Text style={s.actionText}>Try again</Text></Pressable>}
                    <Pressable testID="knowledge-check-submit" accessibilityRole="button" accessibilityState={{disabled:!assessment || !allAnswered || busy,busy}}
                        disabled={!assessment || !allAnswered || busy} onPress={submit} style={[s.primary,(!assessment || !allAnswered || busy)&&s.disabled]}>
                        <Text style={s.actionText}>{busy ? 'Please wait…' : 'Submit answers'}</Text>
                    </Pressable>
                </View>
                </>}
            </View></View>
        </Modal>
    </>;
}

const s=StyleSheet.create({
    action:{minHeight:70,marginTop:36,padding:12,gap:14,flexDirection:'row',alignItems:'center',borderRadius:10,borderWidth:1,borderColor:'#353e44',backgroundColor:'rgba(6,15,20,.9)'},
    disabled:{opacity:.5},icon:{width:42,height:42},copy:{flex:1},title:{fontSize:14,fontWeight:'700',color:'#fff'},subtitle:{fontSize:13,lineHeight:19,color:'#a6a6a4',marginTop:4},
    arrow:{fontSize:28,color:'#d4af53'},scrim:{flex:1,justifyContent:'center',alignItems:'center',padding:20,backgroundColor:'rgba(0,0,0,.78)'},
    resultScrim:{flex:1,alignItems:'center',justifyContent:'center',backgroundColor:'#05090b'},
    dialog:{width:'100%',maxWidth:480,maxHeight:'92%',padding:20,borderRadius:16,borderWidth:1,borderColor:'#353e44',backgroundColor:'#0b151c'},
    fullscreenDialog:{width:'100%',maxWidth:480,height:'100%',maxHeight:'100%',padding:0,borderWidth:0,borderRadius:0,backgroundColor:'#05090b',overflow:'hidden'},
    introDialog:{},
    introBackground:{position:'absolute',top:0,right:0,bottom:0,left:0,width:'100%',height:'100%',opacity:.48},
    introBackgroundShade:{position:'absolute',top:0,right:0,bottom:0,left:0,backgroundColor:'rgba(3,8,12,.48)'},
    introHero:{height:'40%',minHeight:250,maxHeight:380,justifyContent:'center',alignItems:'center',paddingHorizontal:24,overflow:'hidden'},
    introBack:{position:'absolute',top:46,left:20,width:42,height:42,alignItems:'center',justifyContent:'center',zIndex:1},
    introHeroTitle:{fontSize:24,lineHeight:30,fontWeight:'800',textAlign:'center',color:'#fff',marginTop:22},
    introHeroCaption:{position:'absolute',bottom:18,fontSize:11,lineHeight:14,fontWeight:'700',textAlign:'center',color:'#fff'},
    introScroll:{flex:1,marginTop:-1},
    introScrollContent:{flexGrow:1,paddingHorizontal:19,paddingTop:10,paddingBottom:22},
    introEyebrow:{fontSize:9,fontWeight:'700',letterSpacing:1.1,textAlign:'center',color:'#d7b24d'},
    introTitle:{fontSize:20,lineHeight:25,fontWeight:'700',textAlign:'center',color:'#fff',marginTop:6},
    introCopy:{fontSize:10,lineHeight:15,textAlign:'center',color:'#b2b1ac',marginTop:4},
    introStats:{flexDirection:'row',gap:7,marginTop:13},
    introStat:{flex:1,minHeight:62,paddingVertical:6,paddingHorizontal:3,alignItems:'center',justifyContent:'center',borderRadius:9,borderWidth:1,borderColor:'#303a3f',backgroundColor:'rgba(6,15,20,.92)'},
    introStatIcon:{fontSize:14,color:'#d7b24d'},
    introStatValue:{fontSize:14,fontWeight:'700',color:'#fff',marginTop:1},
    introStatLabel:{fontSize:8,color:'#aaa9a4',marginTop:1},
    skillsCard:{padding:11,marginTop:10,borderWidth:1,borderColor:'#303a3f',borderRadius:9,backgroundColor:'rgba(6,15,20,.94)'},
    skillsTitle:{fontSize:11,fontWeight:'700',color:'#fff',marginBottom:6},
    skillBullet:{minHeight:20,flexDirection:'row',alignItems:'center',gap:8},
    skillBulletIcon:{fontSize:13,color:'#d7b24d'},
    skillBulletText:{fontSize:10,color:'#c5c4bf'},
    introPrimary:{minHeight:48,alignItems:'center',justifyContent:'center',marginTop:'auto',paddingHorizontal:12,borderRadius:8,backgroundColor:'#c4872d'},
    introPrimaryText:{fontSize:13,fontWeight:'700',color:'#15110a'},
    introFootnote:{fontSize:9,textAlign:'center',color:'#999893',marginTop:8},
    heading:{fontSize:20,fontWeight:'700',color:'#d4af53',marginBottom:8},questions:{flexShrink:1,marginTop:12},question:{marginBottom:20},
    questionText:{fontSize:15,lineHeight:22,color:'#fff',marginBottom:10},choice:{padding:12,marginBottom:8,borderRadius:9,borderWidth:1,borderColor:'#353e44'},
    choiceSelected:{borderColor:'#d4af53',backgroundColor:'rgba(212,175,83,.12)'},choiceText:{fontSize:14,lineHeight:20,color:'#fff'},
    error:{fontSize:13,lineHeight:19,color:'#f19a86',marginTop:12},
    actions:{flexDirection:'row',justifyContent:'flex-end',gap:10,marginTop:18},primary:{minHeight:46,paddingHorizontal:14,borderRadius:8,alignItems:'center',justifyContent:'center',backgroundColor:'#d4af53'},
    secondary:{minHeight:46,paddingHorizontal:14,borderRadius:8,alignItems:'center',justifyContent:'center',borderWidth:1,borderColor:'#59636a'},
    actionText:{fontSize:14,fontWeight:'700',color:'#000'},
    resultDialog:{height:'100%',maxHeight:'100%',paddingHorizontal:19,paddingTop:38,paddingBottom:20,borderWidth:0,borderRadius:0,backgroundColor:'#080d10',overflow:'hidden'},
    retryDialog:{paddingHorizontal:22,paddingTop:56,paddingBottom:24},
    retryBackground:{position:'absolute',top:0,right:0,bottom:0,left:0,width:'100%',height:'100%'},
    retryShade:{position:'absolute',top:0,right:0,bottom:0,left:0,backgroundColor:'rgba(3,7,12,.62)'},
    retryHeader:{height:42,flexDirection:'row',alignItems:'center',justifyContent:'space-between'},
    retryHeaderTitle:{fontSize:20,fontWeight:'700',color:'#fff'},
    retryMessage:{alignItems:'center',marginTop:42,paddingHorizontal:8},
    retryIcon:{width:60,height:60,alignItems:'center',justifyContent:'center'},
    retryArrow:{position:'absolute',fontSize:31,lineHeight:38,color:'#d7b24d'},
    retryTitle:{fontSize:18,lineHeight:24,fontWeight:'700',textAlign:'center',color:'#fff',marginTop:22},
    retryCopy:{fontSize:13,lineHeight:19,textAlign:'center',color:'#c2c1bd',marginTop:7},
    retryProgress:{flexDirection:'row',alignItems:'center',justifyContent:'space-between',marginTop:42,paddingHorizontal:2},
    retryQuestion:{fontSize:13,fontWeight:'700',color:'#d7b24d'},
    retryNote:{fontSize:11,lineHeight:16,color:'#c0bfba',marginTop:3},
    retryAttempt:{fontSize:12,color:'#c0bfba'},
    retryActions:{gap:10,marginTop:'auto'},
    retryPrimary:{height:54,alignItems:'center',justifyContent:'center',borderRadius:9,backgroundColor:'#f2b82f'},
    retryPrimaryText:{fontSize:14,fontWeight:'700',color:'#17130a'},
    retrySecondary:{height:54,alignItems:'center',justifyContent:'center',borderWidth:1,borderColor:'#42403b',borderRadius:9,backgroundColor:'rgba(20,23,25,.62)'},
    retrySecondaryText:{fontSize:13,fontWeight:'600',color:'#fff'},
    retryLink:{alignSelf:'center',paddingVertical:8},
    retryLinkText:{fontSize:11,fontWeight:'700',color:'#d7b24d',textDecorationLine:'underline'},
    resultBackground:{position:'absolute',top:0,right:0,bottom:0,left:0,width:'100%',height:'100%',opacity:.2},
    resultShade:{position:'absolute',top:0,right:0,bottom:0,left:0,backgroundColor:'rgba(3,7,9,.78)'},
    resultContent:{flex:1},
    resultContentContainer:{flexGrow:1,paddingBottom:12},
    resultHeader:{height:38,flexDirection:'row',alignItems:'center',justifyContent:'space-between'},
    back:{width:32,height:32,alignItems:'center',justifyContent:'center'},
    backText:{fontSize:28,color:'#d7b24d',lineHeight:30},
    resultHeaderTitle:{fontSize:15,fontWeight:'700',color:'#fff'},
    shareIcon:{fontSize:21,fontWeight:'700',color:'#d7b24d'},
    nameBadge:{alignSelf:'center',marginTop:2,paddingHorizontal:9,paddingVertical:3,borderRadius:4,backgroundColor:'#9b382b'},
    nameText:{fontSize:10,fontWeight:'700',color:'#fff'},
    scoreRing:{width:98,height:98,alignSelf:'center',marginTop:12,alignItems:'center',justifyContent:'center'},
    scoreCenter:{position:'absolute',alignItems:'center',justifyContent:'center'},
    scoreValue:{fontSize:20,fontWeight:'700',color:'#fff'},
    scoreCaption:{fontSize:10,color:'#c7b46f',marginTop:1},
    resultTitle:{fontSize:19,fontWeight:'700',textAlign:'center',color:'#fff',marginTop:4},
    resultCopy:{fontSize:11,lineHeight:16,textAlign:'center',color:'#b2b1ac',marginTop:4},
    statusChip:{alignSelf:'center',marginTop:6,paddingHorizontal:10,paddingVertical:3,borderRadius:5,backgroundColor:'rgba(49,190,106,.16)'},
    statusChipText:{fontSize:9,fontWeight:'700',letterSpacing:.7,color:'#49ce7b'},
    statRow:{flexDirection:'row',gap:8,marginTop:12},
    statCard:{flex:1,minHeight:68,paddingVertical:7,alignItems:'center',justifyContent:'center',borderRadius:9,borderWidth:1,borderColor:'#2c2d2a',backgroundColor:'rgba(30,30,28,.75)'},
    statIcon:{fontSize:14,fontWeight:'700'},correctIcon:{color:'#47cb7a'},reviewIcon:{color:'#d7b24d'},skillIcon:{color:'#d7b24d'},
    statValue:{fontSize:15,fontWeight:'700',color:'#fff',marginTop:2},statLabel:{fontSize:9,color:'#aaa9a4',marginTop:1},
    breakdownCard:{padding:12,marginTop:10,borderRadius:10,borderWidth:1,borderColor:'#30302c',backgroundColor:'rgba(29,29,27,.88)'},
    sectionTitle:{fontSize:12,fontWeight:'700',color:'#fff',marginBottom:8},
    skillRow:{flexDirection:'row',justifyContent:'space-between',alignItems:'center'},
    skillName:{fontSize:10,color:'#c5c4bf'},skillScore:{fontSize:10,fontWeight:'700',color:'#d7b24d'},
    track:{height:4,marginTop:5,borderRadius:3,backgroundColor:'#393a37',overflow:'hidden'},
    fill:{height:'100%',borderRadius:3,backgroundColor:'#d7b24d'},
    nextStep:{padding:11,marginTop:8,borderRadius:10,borderWidth:1,borderColor:'#30302c',backgroundColor:'rgba(29,29,27,.85)'},
    nextTitle:{fontSize:11,fontWeight:'700',color:'#fff'},
    nextCopy:{fontSize:10,lineHeight:14,color:'#b2b1ac',marginTop:5},
    resultActions:{gap:8,paddingTop:12},
    resultPrimary:{height:52,width:'100%',alignItems:'center',justifyContent:'center',borderRadius:9,backgroundColor:'#d7b24d'},
    resultPrimaryText:{fontSize:14,fontWeight:'700',color:'#17130a'},
    resultSecondary:{height:52,width:'100%',alignItems:'center',justifyContent:'center',borderWidth:1,borderColor:'#393a37',borderRadius:9,backgroundColor:'rgba(39,39,36,.45)'},
    dashboardText:{fontSize:12,fontWeight:'600',color:'#fff'},
    reviewList:{marginTop:10},reviewCard:{padding:12,marginBottom:8,borderRadius:9,borderWidth:1,borderColor:'#34342f',backgroundColor:'rgba(14,20,22,.92)'},
    reviewQuestion:{fontSize:13,lineHeight:19,fontWeight:'600',color:'#fff',marginBottom:8},
    reviewAnswer:{fontSize:12,lineHeight:18},correctText:{color:'#53ce82'},incorrectText:{color:'#ed8477'},
    reviewCorrect:{fontSize:12,lineHeight:18,color:'#d7b24d',marginTop:5},
});
