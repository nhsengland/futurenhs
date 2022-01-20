import { Page } from '@appTypes/page';
import { GenericPageTextContent } from '@appTypes/content';
import { User } from '@appTypes/user';
import { SearchResult } from '@appTypes/search';

interface Text extends GenericPageTextContent {}

export interface Props extends Page {
    user: User;
    term: string | Array<string>;
    text: Text;
    resultsList: Array<SearchResult>;
}