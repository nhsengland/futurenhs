import { GetServerSidePropsContext } from '@appTypes/next';

export const selectUser = (context: GetServerSidePropsContext) => context.req.user ?? null;
export const selectPagination = (context: GetServerSidePropsContext) => {

    const pagination = {
        pageNumber: context.req.body?.pageNumber ?? 1,
        pageSize: context.req.body?.pageSize ?? 10
    }

    return pagination;

};