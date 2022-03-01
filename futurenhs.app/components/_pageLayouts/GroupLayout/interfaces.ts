import { GroupsPageTextContent } from "@appTypes/content";
import { Image } from "@appTypes/image";

export interface Props {
    tabId: 'index' | 'forum' | 'files' | 'members';
    shouldRenderSearch?: boolean;
    shouldRenderUserNavigation?: boolean;
    shouldRenderPhaseBanner?: boolean;
    shouldRenderBreadCrumb?: boolean; 
    shouldRenderMainNav?: boolean;
    user?: any;
    actions?: any;
    themeId?: string;
    className?: string;
    entityText?: GroupsPageTextContent;
    image?: Image;
    children?: any;
}
