import { Group } from '@appTypes/group'
import { Page } from '@appTypes/page'
import { GenericPageTextContent } from '@appTypes/content'

declare interface ContentText extends GenericPageTextContent {
    signIn: string;
}

export interface Props extends Page {
    contentText: ContentText
}
