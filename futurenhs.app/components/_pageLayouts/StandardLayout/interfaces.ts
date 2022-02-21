import { BreadCrumbList } from '@appTypes/routing';
import { actions } from '@constants/actions';

export interface Props {
    shouldRenderSearch?: boolean;
    shouldRenderUserNavigation?: boolean;
    shouldRenderPhaseBanner?: boolean;
    shouldRenderBreadCrumb?: boolean; 
    shouldRenderMainNav?: boolean;
    user?: any;
    actions?: Array<actions>;
    breadCrumbList?: BreadCrumbList;
    searchTerm?: any;
    className?: string; 
    children: any;
}
