import { User } from '@appTypes/user';

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
        editProfile: string;
        logOut: string;
        logOutHeading: string; 
        logOutBody: string;
        logOutCancel: string;
        logOutConfirm: string;
    };
    user: User;
    shouldRenderSearch?: boolean;
    shouldRenderNavigation?: boolean;
    searchTerm?: any;
    className?: string; 
}
