import { Image } from './image'; 

export interface Group {
    content: {
        mainHeadingHtml: string;
        strapLineText?: string;
    };
    slug?: string; 
    image?: Image;
    totalDiscussionCount?: number; 
    totalMemberCount?: number;    
}
