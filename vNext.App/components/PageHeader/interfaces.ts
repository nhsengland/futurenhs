import { Image } from '@appTypes/image';

export interface Props {
    id: string;
    image?: Image;
    content?: {
        mainHeadingHtml: string;
        descriptionHtml?: string;
        navMenuTitleText: string;   
    };
    shouldRenderActionsMenu?: boolean;
    actionsMenuList?: Array<{
        url: string;
        text: string;
    }>;
    navMenuList?: Array<{
        url: string;
        text: string;
        isActive?: boolean;
    }>;
    className?: string;
}