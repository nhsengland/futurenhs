export interface Discussion {
    text: {
        title: string;
        body?: string;
    };
    discussionId?: string;
    totalViewCount?: number; 
    totalCommentCount?: number;    
}
