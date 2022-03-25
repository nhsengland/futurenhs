import { GenericPageTextContent } from "@appTypes/content";
import { Theme } from "@appTypes/theme";

export interface Props {
    shouldRenderSearch?: boolean;
    shouldRenderUserNavigation?: boolean;
    shouldRenderPhaseBanner?: boolean;
    shouldRenderBreadCrumb?: boolean; 
    shouldRenderMainNav?: boolean;
    user?: any;
    actions?: any;
    theme?: Theme;
    className?: string;
    contentText?: GenericPageTextContent;
    children?: any;
    pageTitle?: string;
}
