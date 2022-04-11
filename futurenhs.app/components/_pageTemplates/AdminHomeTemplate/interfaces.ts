import { Page } from '@appTypes/page'
import { SearchResult } from '@appTypes/search'

export interface Props extends Page {
    term: string | Array<string>
    resultsList: Array<SearchResult>
}
