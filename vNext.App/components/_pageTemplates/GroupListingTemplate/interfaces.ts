import { Page } from '@appTypes/page';
import { GroupsPageContent } from '@appTypes/content';
import { Group } from '@appTypes/group';

export interface Props extends Page {
    content: GroupsPageContent;
    groupsList: Array<Group>;
}