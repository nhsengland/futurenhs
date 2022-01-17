import { Image } from './image';
import { GroupsPageContent } from '@appTypes/content'; 

export interface Group {
    content: GroupsPageContent;
    groupId?: string; 
    image?: Image;
    totalDiscussionCount?: number; 
    totalMemberCount?: number;    
}

export interface GroupMember {
    id: string;
    role: string;
    firstName?: string;
    lastName?: string;
    fullName?: string;
    initials?: string;
    pronouns?: string;
    email: string;
    joinDate: string;
    lastLogInDate: string;
}
