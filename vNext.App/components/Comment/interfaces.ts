import { Image } from '@appTypes/image';

export interface Props {
    commentId: string;
    children?: any;
    image?: Image;
    text: any;
    userProfileLink: string;
    date: string;
    likeCount?: number;
    isLiked?: boolean;
    className?: string;
}

