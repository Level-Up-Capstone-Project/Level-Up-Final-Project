import type {BundledAsset} from '../platform/assets';
/** Defines medal names, descriptions, categories and their artwork. */
export const categories = ['All','Learning','Skills','Career','Special'] as const;
export type BadgeCategory = Exclude<typeof categories[number],'All'>;
export type Badge = {id:string;name:string;description:string;category:BadgeCategory;image:BundledAsset;number:number};
export const badges:Badge[] = [
  {
    "id": "first-ascent",
    "name": "First Ascent",
    "description": "Complete onboarding",
    "category": "Career",
    "image": {uri:'/assets/achievements/d3232.png'},
    "number": 1
  },
  {
    "id": "field-manual",
    "name": "Field Manual",
    "description": "Complete your career profile",
    "category": "Career",
    "image": {uri:'/assets/achievements/9a33a.png'},
    "number": 2
  },
  {
    "id": "intel-analyst",
    "name": "Intel Analyst",
    "description": "Complete your first skills assessment",
    "category": "Skills",
    "image": {uri:'/assets/achievements/23f9c.png'},
    "number": 3
  },
  {
    "id": "pathfinder",
    "name": "Pathfinder",
    "description": "Select your career destination",
    "category": "Career",
    "image": {uri:'/assets/achievements/a6230.png'},
    "number": 4
  },
  {
    "id": "mission-ready",
    "name": "Mission Ready",
    "description": "Analyse your CV",
    "category": "Career",
    "image": {uri:'/assets/achievements/bf491.png'},
    "number": 5
  },
  {
    "id": "recon-specialist",
    "name": "Recon Specialist",
    "description": "Explore career opportunities",
    "category": "Career",
    "image": {uri:'/assets/achievements/0aba7.png'},
    "number": 6
  },
  {
    "id": "trailblazer",
    "name": "Trailblazer",
    "description": "Start your learning path",
    "category": "Learning",
    "image": {uri:'/assets/achievements/78ede.png'},
    "number": 7
  },
  {
    "id": "the-big-five",
    "name": "The Big Five",
    "description": "Master five essential skills",
    "category": "Skills",
    "image": {uri:'/assets/achievements/350ff.png'},
    "number": 8
  },
  {
    "id": "high-ground",
    "name": "High Ground",
    "description": "Achieve a skills milestone",
    "category": "Skills",
    "image": {uri:'/assets/achievements/2411a.png'},
    "number": 9
  },
  {
    "id": "forward-base",
    "name": "Forward Base",
    "description": "Reach 25% career readiness",
    "category": "Career",
    "image": {uri:'/assets/achievements/78ee2.png'},
    "number": 10
  },
  {
    "id": "summit-seeker",
    "name": "Summit Seeker",
    "description": "Complete a learning pathway",
    "category": "Learning",
    "image": {uri:'/assets/achievements/0d9ed.png'},
    "number": 11
  },
  {
    "id": "precision-strike",
    "name": "Precision Strike",
    "description": "Pass an assessment",
    "category": "Skills",
    "image": {uri:'/assets/achievements/fdd12.png'},
    "number": 12
  },
  {
    "id": "iron-will",
    "name": "Iron Will",
    "description": "Maintain a learning streak",
    "category": "Learning",
    "image": {uri:'/assets/achievements/54334.png'},
    "number": 13
  },
  {
    "id": "strategic-command",
    "name": "Strategic Command",
    "description": "Build your career strategy",
    "category": "Career",
    "image": {uri:'/assets/achievements/883bf.png'},
    "number": 14
  },
  {
    "id": "elite-operator",
    "name": "Elite Operator",
    "description": "Earn five achievements",
    "category": "Special",
    "image": {uri:'/assets/achievements/203c1.png'},
    "number": 15
  },
  {
    "id": "beyond-limits",
    "name": "Beyond Limits",
    "description": "Exceed a learning target",
    "category": "Learning",
    "image": {uri:'/assets/achievements/b54a2.png'},
    "number": 16
  },
  {
    "id": "summit-command",
    "name": "Summit Command",
    "description": "Reach 100% career readiness",
    "category": "Career",
    "image": {uri:'/assets/achievements/b8126.png'},
    "number": 17
  },
  {
    "id": "mission-accomplished",
    "name": "Mission Accomplished",
    "description": "Secure your first career opportunity",
    "category": "Career",
    "image": {uri:'/assets/achievements/3b227.png'},
    "number": 18
  }
];
