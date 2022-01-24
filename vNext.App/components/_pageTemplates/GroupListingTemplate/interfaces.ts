import { Page } from '@appTypes/page';
import { GroupsPageTextContent } from '@appTypes/content';
import { Group } from '@appTypes/group';

export interface Props extends Page {
    text: GroupsPageTextContent;
    isGroupMember: boolean;
    groupsList: Array<Group>;
}