import { Image } from './image';
import { Member } from '@appTypes/member'; 
import { GroupsPageTextContent } from '@appTypes/content'; 

export interface Group {
    text: GroupsPageTextContent;
    groupId?: string; 
    themeId?: string;
    imageId?: string;
    image?: Image;
    totalDiscussionCount?: number; 
    totalMemberCount?: number;
    owner?: Partial<User>;
    isDeleted?: boolean;
}

export interface GroupMember extends Member {}
