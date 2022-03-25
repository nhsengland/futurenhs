import { ApiPaginatedResponse } from '@appTypes/service';
import { Pagination } from '@appTypes/pagination';

declare interface Config {
    apiPaginatedResponse: ApiPaginatedResponse<any>;
}

export const getClientPaginationFromApi = ({
    apiPaginatedResponse
}: Config): Pagination => {

    try {

        const { offset, limit, totalRecords } = apiPaginatedResponse;
    
        return {
            pageNumber: offset && limit ? (offset / limit) + 1 : 1,
            pageSize: limit ?? null,
            totalRecords: totalRecords ?? null
        };

    } catch(error){

        return null;

    }

};
