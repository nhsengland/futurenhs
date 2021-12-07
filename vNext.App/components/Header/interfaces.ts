import { User } from '@appTypes/user';

export interface Props {
    skipLinkList?: Array<{
        id: string;
        text: string;
    }>;
    content: { 
        editProfileText: string;
        logOutText: string;
        logOutHeadingText: string; 
        logOutBodyText: string;
        logOutCancelText: string;
        logOutConfirmText: string;
    };
    user: User;
    shouldRenderSearch?: boolean;
    shouldRenderNavigation?: boolean; 
    className?: string; 
}
