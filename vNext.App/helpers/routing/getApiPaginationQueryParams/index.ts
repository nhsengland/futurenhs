import { Pagination } from '@appTypes/pagination';

declare interface Config {
    pagination: Pagination;
}

export const getApiPaginationQueryParams = ({
    pagination
}: Config): string => {

    try {

        const { pageNumber, pageSize } = pagination;

        const offset: number = (pageNumber - 1) * pageSize;
        const limit: number = pageSize;

        const route: string = `offset=${offset}&limit=${limit}`;
    
        return route;

    } catch(error){

        return '';

    }

};
