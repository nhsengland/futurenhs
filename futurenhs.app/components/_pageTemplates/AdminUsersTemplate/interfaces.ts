import { Page } from '@appTypes/page';
import { GenericPageTextContent } from '@appTypes/content';
import { Member } from '@appTypes/member';

declare interface ContentText extends GenericPageTextContent {
    usersHeading: string;
    noUsers: string;
    createUser: string;
}

export interface Props extends Page {
    usersList: Array<Member>;
    contentText: ContentText;
}