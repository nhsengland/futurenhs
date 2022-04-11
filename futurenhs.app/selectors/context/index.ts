import { GetServerSidePropsContext } from '@appTypes/next'

import { routeParams } from '@constants/routes'
import {
    getServerSideFormData,
    ServerSideFormData,
    getServerSideMultiPartFormData,
} from '@helpers/util/form'

import { User } from '@appTypes/user'
import { Pagination } from '@appTypes/pagination'

export const selectProps = (context: GetServerSidePropsContext): any =>
    context.props ?? {}
export const selectRequestMethod = (context: GetServerSidePropsContext): any =>
    context.req.method
export const selectLocale = (context: GetServerSidePropsContext): string =>
    context.req?.locale?.() ?? ''
export const selectCsrfToken = (context: GetServerSidePropsContext): string =>
    context.req?.csrfToken?.() ?? ''
export const selectUser = (context: GetServerSidePropsContext): User =>
    context.req?.user ?? null
export const selectFormData = (
    context: GetServerSidePropsContext
): ServerSideFormData =>
    context.req?.body && Object.keys(context.req.body).length > 0
        ? getServerSideFormData(context.req.body)
        : null
export const selectMultiPartFormData = (
    context: GetServerSidePropsContext
): any =>
    context.req?.body && Object.keys(context.req.body).length > 0
        ? getServerSideMultiPartFormData(context.req.body)
        : null
export const selectParam = (
    context: GetServerSidePropsContext,
    paramName: routeParams
): string => (context.params as any)?.[paramName] ?? null
export const selectQuery = (
    context: GetServerSidePropsContext,
    queryName: string
): string =>
    context.req?.query?.[queryName]
        ? decodeURIComponent(context.req?.query?.[queryName])
        : null
export const selectPagination = (
    context: GetServerSidePropsContext
): Pagination => ({
    pageNumber: selectQuery(context, 'pageNumber')
        ? parseInt(decodeURIComponent(selectQuery(context, 'pageNumber')), 10)
        : null,
    pageSize: selectQuery(context, 'pageSize')
        ? parseInt(decodeURIComponent(selectQuery(context, 'pageSize')), 10)
        : null,
})
