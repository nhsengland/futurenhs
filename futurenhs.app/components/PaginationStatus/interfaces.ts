import { Pagination } from '@appTypes/pagination';

export interface Props extends Pagination {
    text?: {
        prefix: string; 
        infix: string;
        suffix: string;  
    };
    shouldEnableLoadMore?: boolean;
}