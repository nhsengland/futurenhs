export interface Props {
    id: string;
    iconName?: string;
    likeCount: number;
    isLiked?: boolean;
    likeAction: any;
    text: {
        countSingular: string;
        countPlural: string;
    };
    className?: string;
}

