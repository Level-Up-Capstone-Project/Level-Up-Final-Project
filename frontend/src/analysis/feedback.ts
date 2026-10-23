/** Defines the text and section formats accepted by CV feedback reports. */
export type CvFeedbackSection = {
    heading: string;
    body?: string;
    bullets?: string[];
};

// The backend may return the model's complete text or structured report sections.
export type CvFeedback = string | {
    title?: string;
    introduction?: string;
    sections: CvFeedbackSection[];
    conclusion?: string;
};
