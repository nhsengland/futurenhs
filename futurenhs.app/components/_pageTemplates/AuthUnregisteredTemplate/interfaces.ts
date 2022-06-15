import { Page } from '@appTypes/page'
import { GenericPageTextContent } from '@appTypes/content'

declare interface ContentText extends GenericPageTextContent {
    signOut: string;
}

export interface Props extends Page {
    contentText: ContentText
}
