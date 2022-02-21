import { User } from '@appTypes/user';
import { actions } from '@constants/actions';

export interface Props {
    skipLinkList?: Array<{
        id: string;
        text: string;
    }>;
    navMenuList: Array<{
        url: string;
        text: string;
        isActive?: boolean;
        meta?: {
            themeId?: number | string;
            iconName?: string;
        }
    }>;
    text: { 
        admin: string;
        editProfile: string;
        logOut: string;
        logOutHeading: string; 
        logOutBody: string;
        logOutCancel: string;
        logOutConfirm: string;
    };
    user: User;
    actions?: Array<actions>;
    shouldRenderSearch?: boolean;
    shouldRenderNavigation?: boolean;
    searchTerm?: any;
    className?: string; 
}
