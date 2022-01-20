import { GroupPage } from '@appTypes/page';
import { GroupMember } from '@appTypes/group';
import { GroupsPageTextContent } from '@appTypes/content';

declare interface Text extends GroupsPageTextContent {
    pendingMemberRequestsHeading: string; 
    membersHeading: string;
}

export interface Props extends GroupPage {
    text: Text;
    pendingMembers: Array<any>;
    members: Array<GroupMember>;
}