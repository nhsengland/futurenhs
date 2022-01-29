export interface Discussion {
    text: {
        title: string;
        body?: string;
    };
    discussionId?: string;
    responseCount?: number; 
    viewCount?: number;    
}
