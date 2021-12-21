import { Image } from './image';
import { GroupsPageContent } from '@appTypes/content'; 

export interface Group {
    content: GroupsPageContent;
    slug?: string; 
    image?: Image;
    totalDiscussionCount?: number; 
    totalMemberCount?: number;    
}

export interface GroupMember {
    id: string;
    role: string;
    fullName: string;
    email: string;
    joinDate: string;
    lastLogInDate: string;
}
