/** Defines opportunity records, working models and employment types. */
export type WorkModel = 'Remote' | 'Hybrid' | 'On-site' | 'Unspecified';
export type EmploymentType = 'Contract' | 'Full-time' | 'Part-time' | 'Unspecified';
export type Opportunity = { id: string; company: string; title: string; area: string; region: string; workModel: WorkModel; employmentType: EmploymentType; match: number | null; description: string; applicationUrl?: string; requiredSkills?: { name: string; status: 'matched' | 'partial' | 'missing' }[]; careerIntel?: string };
