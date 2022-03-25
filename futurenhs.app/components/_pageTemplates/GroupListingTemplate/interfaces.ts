import { Page } from '@appTypes/page';
import { Group } from '@appTypes/group';

export interface Props extends Page {
    isGroupMember: boolean;
    groupsList: Array<Group>;
}