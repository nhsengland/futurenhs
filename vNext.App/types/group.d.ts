import { Image } from './image';
import { GroupsPageTextContent } from '@appTypes/content'; 

export interface Group {
    text: GroupsPageTextContent;
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
