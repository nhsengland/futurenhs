import { Group } from '@appTypes/group';
import { Page } from '@appTypes/page';
import { GenericPageTextContent } from '@appTypes/content';

declare interface ContentText extends GenericPageTextContent {
    usersHeading: string;
    noGroups: string;
    createGroup: string;
}

export interface Props extends Page {
    groupsList: Array<Group>;
    contentText: ContentText;
}