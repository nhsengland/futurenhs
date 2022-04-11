import { GenericPageTextContent } from '@appTypes/content'
import { Page } from '@appTypes/page'

declare interface ContentText extends GenericPageTextContent {
    firstNameLabel: string
    lastNameLabel: string
    pronounsLabel: string
    emailLabel: string
}

export interface Props extends Page {
    siteUser: any
    contentText: ContentText
}
