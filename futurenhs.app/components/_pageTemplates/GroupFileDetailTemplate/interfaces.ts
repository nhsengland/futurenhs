import { FolderContent } from '@appTypes/file'
import { GroupPage } from '@appTypes/page'
import { GroupsPageTextContent } from '@appTypes/content'

declare interface ContentText extends GroupsPageTextContent {
    createdByLabel?: string
}

export interface Props extends GroupPage {
    contentText: ContentText
    fileId: string
    file: FolderContent
}
