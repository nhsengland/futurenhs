import { Page } from '@appTypes/page';
import { SearchResult } from '@appTypes/search';

export interface Props extends Page {
    term: string | Array<string>;
    minLength: number;
    resultsList: Array<SearchResult>;
}