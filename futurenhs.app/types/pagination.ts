export interface ApiPagination {
    offset?: number;
    limit?: number;
    totalPages?: number;
    totalRecords?: number;
    firstPage?: URL;
    lastPage?: URL;
    nextPage?: URL;
    previousPage?: URL;
}

export interface Pagination {
    pageNumber?: number;
    pageSize?: number;
    totalRecords?: number;
    firstPage?: URL;
    lastPage?: URL;
    nextPage?: URL;
    previousPage?: URL;
}
 