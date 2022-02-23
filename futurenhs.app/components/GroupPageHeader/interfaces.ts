import { Image } from '@appTypes/image';

export interface Props {
    id: string;
    themeId?: string;
    image?: Image;
    text?: {
        mainHeading: string;
        description?: string;
        navMenuTitle: string;   
    };
    shouldRenderActionsMenu?: boolean;
    actionsMenuList?: Array<{
        id: string;
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