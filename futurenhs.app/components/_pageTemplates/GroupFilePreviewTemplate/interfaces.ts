import { FolderContent } from '@appTypes/file'
import { GroupPage } from '@appTypes/page'
import { GroupsPageTextContent } from '@appTypes/content'
import { CollaboraConnectionParams } from '@appTypes/collabora'

declare interface ContentText extends GroupsPageTextContent {
    previewLabel?: string
    createdByLabel?: string
}

export interface Props extends GroupPage {
    contentText: ContentText
    fileId: string
    file: FolderContent
    preview: CollaboraConnectionParams
    shouldRenderGroupHeader?: boolean
    shouldRenderPhaseBanner?: boolean
    shouldRenderBreadCrumb?: boolean
}
