import { Page } from '@appTypes/page';
import { GenericPageContent } from '@appTypes/content';
import { User } from '@appTypes/user';
import { SearchResult } from '@appTypes/search';

interface Content extends GenericPageContent {}

export interface Props extends Page {
    user: User;
    term: string | Array<string>;
    content: Content;
    resultsList: Array<SearchResult>;
}