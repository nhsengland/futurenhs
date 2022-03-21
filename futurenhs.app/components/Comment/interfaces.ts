import { Image } from '@appTypes/image';
import { DiscussionComment } from '@appTypes/discussion';

export interface Props {
    id?: string;
    csrfToken: string;
    initialErrors?: any;
    commentId: string;
    originComment?: DiscussionComment;
    children?: any;
    image?: Image;
    text: any;
    userProfileLink: string;
    date: string;
    shouldEnableReplies?: boolean;
    replyValidationFailAction?: any;
    replySubmitAction: any;
    shouldEnableLikes?: boolean;
    likeAction?: any;
    likeCount?: number;
    isLiked?: boolean;
    className?: string;
}

