import { GroupsPageTextContent } from "@appTypes/content";
import { Image } from "@appTypes/image";
import { Theme } from "@appTypes/theme";

export interface Props {
    id: 'index' | 'forum' | 'files' | 'members';
    shouldRenderSearch?: boolean;
    shouldRenderUserNavigation?: boolean;
    shouldRenderPhaseBanner?: boolean;
    shouldRenderBreadCrumb?: boolean; 
    shouldRenderMainNav?: boolean;
    user?: any;
    actions?: any;
    theme?: Theme;
    className?: string;
    text?: GroupsPageTextContent;
    image?: Image;
    children?: any;
}
