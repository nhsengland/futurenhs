import { GroupPage } from '@appTypes/page';
import { Discussion, DiscussionComment } from '@appTypes/discussion';
import { Pagination } from '@appTypes/pagination';

export interface Props extends GroupPage {
    discussionId: string;
    discussion: Discussion;
    discussionComments: Array<DiscussionComment>;
    discussionCommentReplies: Record<string, {
        data: Array<DiscussionComment>;
        pagination: Pagination;
    }>
}