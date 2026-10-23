/** Unlock only goals supported by recorded actions or backend completion evidence. */
export type MedalEvidence = {
    onboardingCompleted?: boolean;
    profileCompleted?: boolean;
    targetCareerSelected?: boolean;
    cvAnalysed?: boolean;
    opportunitiesExplored?: boolean;
    readiness?: number | null;
    verifiedSkills?: number;
    roadmapCreated?: boolean;
    roadmapStarted?: boolean;
    roadmapCompleted?: boolean;
};

export function earnedMedals(evidence: MedalEvidence): string[] {
    const ids: string[] = [];
    if (evidence.onboardingCompleted) ids.push('first-ascent');
    if (evidence.profileCompleted) ids.push('field-manual');
    if (evidence.targetCareerSelected) ids.push('pathfinder');
    if (evidence.cvAnalysed) ids.push('mission-ready');
    if (evidence.opportunitiesExplored) ids.push('recon-specialist');
    if ((evidence.verifiedSkills ?? 0) > 0) ids.push('intel-analyst', 'precision-strike');
    if ((evidence.verifiedSkills ?? 0) >= 5) ids.push('the-big-five');
    if (evidence.roadmapCreated) ids.push('strategic-command');
    if (evidence.roadmapStarted) ids.push('trailblazer');
    if (evidence.roadmapCompleted) ids.push('summit-seeker');
    const readiness = evidence.readiness;
    if (typeof readiness === 'number' && Number.isFinite(readiness) && readiness >= 0 && readiness <= 100) {
        if (readiness >= 25) ids.push('forward-base');
        if (readiness === 100) ids.push('summit-command');
    }
    if (ids.length >= 5) ids.push('elite-operator');
    // Streaks, unspecified milestones and secured employment need evidence that is not yet available.
    return ids;
}
