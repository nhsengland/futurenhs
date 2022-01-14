import { GroupPage } from '@appTypes/page';
import { GroupMember } from '@appTypes/group';

export interface Props extends GroupPage {
    pendingMembers: Array<any>;
    members: Array<GroupMember>;
}