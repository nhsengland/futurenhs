import { GetServerSideProps } from 'next'
import { useState, useMemo, useContext } from 'react'
import classNames from 'classnames'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { layoutIds } from '@constants/routes'
import { actions } from '@constants/actions'
import { withUser } from '@helpers/hofs/withUser'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { withRoutes } from '@helpers/hofs/withRoutes'
import {
    selectUser,
    selectPagination,
    selectPageProps,
} from '@helpers/selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { Pagination } from '@appTypes/pagination'
import { dateTime } from '@helpers/formatters/dateTime'
import { actions as actionConstants } from '@constants/actions'
import { Link } from '@components/generic/Link'
import { PaginationWithStatus } from '@components/generic/PaginationWithStatus'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { DynamicListContainer } from '@components/layouts/DynamicListContainer'
import { DataGrid } from '@components/layouts/DataGrid'
import { Page } from '@appTypes/page'
import { GenericPageTextContent } from '@appTypes/content'
import { ClickLink } from '@components/generic/ClickLink'
import { getDomains } from '@services/getDomains'
import { Domain } from '@appTypes/domain'
import { deleteDomain } from '@services/deleteDomain'
import { useNotification } from '@helpers/hooks/useNotification'
import { notifications } from '@constants/notifications'
import { NotificationsContext } from '@helpers/contexts'

declare interface ContentText extends GenericPageTextContent {
    mainHeading: string
    noDomains: string
    addDomain: string
}

export interface Props extends Page {
    domainsList: Array<Domain>
    contentText: ContentText
}

/**
 * Admin users dashboard template
 */
export const AdminDomainsPage: (props: Props) => JSX.Element = ({
    contentText,
    actions,
    pagination,
    routes,
    domainsList = [{ domain: 'test', dateAdded: 'testtt' }],
    user,
}) => {
    const [dynamicDomainsList, setDomainsList] = useState(domainsList)
    const [dynamicPagination, setPagination] = useState(pagination)
    const notificationsContext: any = useContext(NotificationsContext)

    const { secondaryHeading, noDomains, addDomain } = contentText ?? {}
    const handleDeleteDomain = async (domain: string) => {
        debugger
        try {
            const res = await deleteDomain({
                domain,
                user,
            })
            useNotification({
                notificationsContext,
                text: {
                    heading: notifications.SUCCESS,
                    body: `Removed ${domain} from the domain allow list.`,
                },
            })
            return Promise.resolve({})
        } catch (error) {}
    }
    const hasDomains: boolean = true
    const shouldEnableLoadMore: boolean = true
    const shouldRenderAddDomainLink: boolean =
        true ?? actions.includes(actionConstants.SITE_ADMIN_DOMAINS_ADD)

    const columnList = [
        {
            children: 'Domain',
            className: '',
        },
        {
            children: 'Date added',
            className: '',
        },
        {
            children: `Actions`,
            className: 'tablet:u-text-right',
        },
    ]

    const rowList = useMemo(
        () =>
            dynamicDomainsList.map(({ domain, dateAdded }) => {
                const generatedCellClasses = {
                    domain: classNames({
                        ['u-justify-between u-w-full tablet:u-w-1/4 o-truncated-text-lines-1']:
                            true,
                    }),
                    dateAdded: classNames({
                        ['u-justify-between u-w-full tablet:u-w-1/4']: true,
                    }),
                }

                const generatedHeaderCellClasses = {
                    domain: classNames({
                        ['u-text-bold']: true,
                    }),
                    dateAdded: classNames({
                        ['u-text-bold']: true,
                    }),
                }

                const rows = [
                    {
                        children: <span>{domain}</span>,
                        className: generatedCellClasses.domain,
                        headerClassName: generatedHeaderCellClasses.domain,
                    },
                    {
                        children: `${dateTime({ value: dateAdded })}`,
                        className: generatedCellClasses.dateAdded,
                        headerClassName: generatedHeaderCellClasses.dateAdded,
                        shouldRenderCellHeader: true,
                    },
                    {
                        children: (
                            <ClickLink
                                onClick={() => {
                                    handleDeleteDomain(domain)
                                }}
                                text={{
                                    body: 'Delete',
                                    ariaLabel: `Delete domain ${domain}`,
                                }}
                                iconName="icon-delete"
                            />
                        ),
                        className:
                            'u-w-full tablet:u-w-1/8 tablet:u-text-right',
                        headerClassName: 'u-hidden',
                    },
                ]

                return rows
            }),
        []
    )

    /**
     * Handle client-side pagination
     */
    const handleGetPage = async ({
        pageNumber: requestedPageNumber,
        pageSize: requestedPageSize,
    }) => {
        const { data: additionalDomains, pagination } = await getDomains({
            user: user,
            pagination: {
                pageNumber: requestedPageNumber,
                pageSize: requestedPageSize,
            },
        })

        setDomainsList([...dynamicDomainsList, ...additionalDomains])
        setPagination(pagination)
    }

    return (
        <>
            <LayoutColumnContainer className="u-w-full u-flex u-flex-col tablet:u-flex-row-reverse">
                {shouldRenderAddDomainLink && (
                    <LayoutColumn tablet={3} className="c-page-body">
                        <Link href={routes.adminDomainsAdd}>
                            <a className="c-button u-w-full">{addDomain}</a>
                        </Link>
                    </LayoutColumn>
                )}
                <LayoutColumn tablet={9} className="c-page-body">
                    <h2 className="nhsuk-heading-l">{secondaryHeading}</h2>
                    {hasDomains ? (
                        <DynamicListContainer
                            containerElementType="div"
                            shouldEnableLoadMore={shouldEnableLoadMore}
                            className="u-list-none u-p-0"
                        >
                            <DataGrid
                                id="admin-table-domains"
                                columnList={columnList}
                                rowList={rowList}
                                text={{
                                    caption: 'Allowed domains',
                                }}
                            />
                        </DynamicListContainer>
                    ) : (
                        <p>{noDomains}</p>
                    )}
                    <PaginationWithStatus
                        id="group-list-pagination"
                        shouldEnableLoadMore={shouldEnableLoadMore}
                        getPageAction={handleGetPage}
                        {...dynamicPagination}
                    />
                </LayoutColumn>
            </LayoutColumnContainer>
        </>
    )
}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (
    context: GetServerSidePropsContext
) =>
    await pipeSSRProps(
        context,
        {
            routeId: 'bf7bb95a-945a-4c44-99d8-90d19774eddb',
        },
        [withUser, withRoutes, withTextContent],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get data from request context
             */
            const props: Partial<Props> = selectPageProps(context)
            const user: User = selectUser(context)
            const pagination: Pagination = {
                pageNumber: selectPagination(context).pageNumber ?? 1,
                pageSize: selectPagination(context).pageSize ?? 20,
            }

            props.layoutId = layoutIds.ADMIN

            if (!props.actions.includes(actions.SITE_ADMIN_VIEW)) {
                return {
                    notFound: true,
                }
            }

            /**
             * Get data from services
             */
            try {
                const res = await getDomains({ user, pagination })

                props.domainsList = res.data ?? []
                props.pagination = res.pagination
            } catch (error) {
                return handleSSRErrorProps({ props, error })
            }

            /**
             * Return data to page template
             */
            return handleSSRSuccessProps({ props, context })
        }
    )

/**
 * Export page template
 */
export default AdminDomainsPage
