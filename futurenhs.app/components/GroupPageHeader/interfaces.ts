import { actions } from '@constants/actions';
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
    navMenuList?: Array<{
        url: string;
        text: string;
        isActive?: boolean;
    }>;
    actions?: Array<actions>;
    shouldRenderActionsMenu?: boolean;
    className?: string;
}