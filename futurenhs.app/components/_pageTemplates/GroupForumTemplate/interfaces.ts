import { GroupPage } from '@appTypes/page';
import { Discussion } from '@appTypes/discussion';
import { GroupsPageTextContent } from '@appTypes/content';

declare interface ContentText extends GroupsPageTextContent {
    discussionsHeading: string; 
    noDiscussions: string;
    createDiscussion: string;
    createdByLabel?: string;
    lastCommentLabel?: string;
    stickyLabel?: string;
}

export interface Props extends GroupPage {
    contentText: ContentText;
    discussionsList: Array<Discussion>;
}