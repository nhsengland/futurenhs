import { GroupPage } from '@appTypes/page'
import { Discussion, DiscussionComment } from '@appTypes/discussion'
import { GroupsPageTextContent } from '@appTypes/content'

declare interface ContentText extends GroupsPageTextContent {
    createdByLabel?: string
    lastCommentLabel?: string
    totalRecordsLabel?: string
    viewCountLabel?: string
    moreRepliesLabel?: string
    fewerRepliesLabel?: string
    signedInLabel?: string
}

export interface Props extends GroupPage {
    contentText: ContentText
    discussionId: string
    discussion: Discussion
    discussionCommentsList: Array<DiscussionComment>
}
