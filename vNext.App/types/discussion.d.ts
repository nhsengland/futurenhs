import { User } from "./user";

export interface Discussion {
    text: {
        title: string;
        body?: string;
    };
    discussionId?: string;
    responseCount?: number; 
    viewCount?: number;
    createdBy?: Partial<User>;
    createdBy?: Partial<User>;
    created?: string;
    modifiedBy?: Partial<User>;
    modified?: string;
    isSticky?: boolean;
}
