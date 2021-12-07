import { Image } from '@appTypes/image';

export interface Props {
    id: string;
    image?: Image;
    content?: {
        mainHeadingHtml: string;
        descriptionHtml?: string;
        navMenuTitleText: string;   
    };
    navMenuList?: Array<{
        url: string;
        text: string;
    }>;
    className?: string;
}