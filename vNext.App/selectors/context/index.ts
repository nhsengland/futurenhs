import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';
import { Pagination } from '@appTypes/pagination';

export const selectLocale = (context: GetServerSidePropsContext): string => context.req?.locale?.() ?? '';
export const selectCsrfToken = (context: GetServerSidePropsContext): string => context.req?.csrfToken?.() ?? '';
export const selectUser = (context: GetServerSidePropsContext): User => context.req?.user ?? null;
export const selectFolderId = (context: GetServerSidePropsContext): string => (context.params as any)?.folderId ?? null;
export const selectFileId = (context: GetServerSidePropsContext): string => (context.params as any)?.fileId ?? null;
export const selectSearchTerm = (context: GetServerSidePropsContext): string => context.req?.query?.term ? decodeURIComponent(context.req?.query?.term) : null;
export const selectPagination = (context: GetServerSidePropsContext): Pagination => {

    const pagination = {
        pageNumber: context.req.query?.pageNumber ? parseInt(decodeURIComponent(context.req?.query.pageNumber), 10) : null,
        pageSize: context.req.query?.pageSize ? parseInt(decodeURIComponent(context.req?.query.pageSize), 10) : null
    }

    return pagination;

};
