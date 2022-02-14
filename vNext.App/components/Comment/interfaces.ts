import { Image } from '@appTypes/image';

export interface Props {
    csrfToken: string;
    commentId: string;
    children?: any;
    image?: Image;
    text: any;
    userProfileLink: string;
    date: string;
    shouldEnableReplies?: boolean;
    replyChangeAction: any;
    replySubmitAttemptAction: any;
    replySubmitAction: any;
    shouldEnableLikes?: boolean;
    likeAction?: any;
    likeCount?: number;
    isLiked?: boolean;
    className?: string;
}

