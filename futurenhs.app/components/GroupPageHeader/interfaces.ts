import { Image } from '@appTypes/image';
import { Theme } from '@appTypes/theme';

export interface Props {
    id: string;
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
    theme?: Theme;
    className?: string;
}