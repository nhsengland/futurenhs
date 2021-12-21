import { Pagination } from '@appTypes/pagination';

export interface Props extends Pagination {
    content?: {
        prefixText: string; 
        infixText: string;
        suffixText: string;  
    };
    shouldEnableLoadMore?: boolean;
}