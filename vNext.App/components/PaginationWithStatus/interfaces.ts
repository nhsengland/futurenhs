import { Pagination } from '@appTypes/pagination';

export interface Props extends Pagination {
    id: string;
    content?: {
        loadMoreText?: string;
        previousText?: string;
        nextText?: string;    
    };
    visiblePages?: number;
    shouldEnableLoadMore?: boolean;
    shouldDisable?: boolean;
    getPageAction?: (config: {
        pageNumber: number;
        pageSize: number;
    }) => any;
    className?: string;
}