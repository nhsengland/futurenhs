import { Pagination } from '@appTypes/pagination';

declare interface Config {
    pagination: Pagination;
    defaults: Pagination;
}

export const getApiPaginationQueryParams = ({
    pagination,
    defaults
}: Config): string => {

    try {

        const pageNumber = pagination?.pageNumber ?? defaults?.pageNumber;
        const pageSize = pagination?.pageSize ?? defaults?.pageSize;

        const offset: number = (pageNumber - 1) * pageSize;
        const limit: number = pageSize ?? 10;

        const route: string = `offset=${offset}&limit=${limit}`;
    
        return route;

    } catch(error){

        console.log(error);

        return '';

    }

};
