import { GroupPage } from '@appTypes/page';
import { GroupMember } from '@appTypes/group';
import { GroupsPageTextContent } from '@appTypes/content';

declare interface ContentText extends GroupsPageTextContent {
    pendingMemberRequestsHeading: string; 
    membersHeading: string;
    noPendingMembers: string;
    noMembers: string;
}

export interface Props extends GroupPage {
    contentText: ContentText;
    pendingMembers: Array<any>;
    members: Array<GroupMember>;
}