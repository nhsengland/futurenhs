import { GetServerSidePropsContext } from 'next';
import { Group } from '@appTypes/group';
import { Content } from '@appTypes/content';

export interface ServiceResponse<T> {
    data?: T;
    errors?: Record<string, any>;
    succeeded?: boolean;
    message?: string;
}

export interface ServicePaginatedResponse<T> {
    data?: T;
    errors?: Record<string, any>;
    succeeded?: boolean;
    message?: string;
    pageNumber?: number;
    pageSize?: number;
    firstPage?: URL;
    lastPage?: URL;
    totalPages?: number;
    totalRecords?: number;
    nextPage?: URL;
    previousPage?: URL;
}
 