import { GroupsPageContent } from "@appTypes/content";
import { Image } from "@appTypes/image";

export interface Props {
    id: 'index' | 'forum' | 'files' | 'members';
    shouldRenderSearch?: boolean;
    shouldRenderUserNavigation?: boolean;
    shouldRenderPhaseBanner?: boolean;
    shouldRenderBreadCrumb?: boolean; 
    shouldRenderMainNav?: boolean;
    user?: any;
    className?: string;
    content?: GroupsPageContent;
    image?: Image;
    children?: any;
}
