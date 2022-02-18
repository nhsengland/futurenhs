import { GroupPage } from '@appTypes/page';
import { Discussion, DiscussionComment } from '@appTypes/discussion';

export interface Props extends GroupPage {
    discussionId: string;
    discussion: Discussion;
    discussionCommentsList: Array<DiscussionComment>;
}