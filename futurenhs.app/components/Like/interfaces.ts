export interface Props {
    targetId: string;
    iconName?: string;
    likeCount: number;
    isLiked?: boolean;
    shouldEnable?: boolean;
    likeAction: any;
    text: {
        countSingular: string;
        countPlural: string;
        like: string;
        removeLike: string;
    };
    className?: string;
}

