import { Page } from '@appTypes/page';
import { GenericPageTextContent } from '@appTypes/content';
import { SearchResult } from '@appTypes/search';

declare interface ContentText extends GenericPageTextContent {
    usersHeading: string;
    noUsers: string;
    createUser: string;
}

export interface Props extends Page {
    usersList: Array<SearchResult>;
    contentText: ContentText;
}