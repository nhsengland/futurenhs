import { GroupsPageTextContent } from "@appTypes/content";
import { Image } from "@appTypes/image";
import { Routes } from "@appTypes/routing";
import { User } from "@appTypes/user";

export interface Props {
    tabId: 'index' | 'forum' | 'files' | 'members';
    routes: Routes;
    shouldRenderSearch?: boolean;
    shouldRenderUserNavigation?: boolean;
    shouldRenderPhaseBanner?: boolean;
    shouldRenderBreadCrumb?: boolean; 
    shouldRenderMainNav?: boolean;
    shouldRenderGroupHeader?: boolean;
    user?: User;
    actions?: any;
    themeId?: string;
    className?: string;
    entityText?: GroupsPageTextContent;
    image?: Image;
    children?: any;
}
