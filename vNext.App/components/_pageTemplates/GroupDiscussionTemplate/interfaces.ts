import { GroupPage } from '@appTypes/page';
import { Discussion, DiscussionComment } from '@appTypes/discussion';

export interface Props extends GroupPage {
    discussionId: string;
    discussion: Discussion;
    discussionComments: Array<DiscussionComment>;
}