import { Page } from '@appTypes/page';
import { GenericPageTextContent } from '@appTypes/content';
import { Member } from '@appTypes/member';

declare interface ContentText extends GenericPageTextContent {
    mainHeading: string;
    noUsers: string;
    inviteUser: string;
}

export interface Props extends Page {
    usersList: Array<Member>;
    contentText: ContentText;
}