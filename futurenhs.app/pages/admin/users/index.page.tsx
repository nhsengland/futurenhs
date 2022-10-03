import { GetServerSideProps } from 'next'
import { useState, useMemo } from 'react'
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
import { getSiteUsers } from '@services/getSiteUsers'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { Pagination } from '@appTypes/pagination'
import { capitalise } from '@helpers/formatters/capitalise'
import { dateTime } from '@helpers/formatters/dateTime'
import { actions as actionConstants } from '@constants/actions'
import { Link } from '@components/generic/Link'
import { ActionLink } from '@components/generic/ActionLink'
import { PaginationWithStatus } from '@components/generic/PaginationWithStatus'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { DynamicListContainer } from '@components/layouts/DynamicListContainer'
import { DataGrid } from '@components/layouts/DataGrid'
import { Page } from '@appTypes/page'
import { GenericPageTextContent } from '@appTypes/content'
import { Member } from '@appTypes/member'

declare interface ContentText extends GenericPageTextContent {
    mainHeading: string
    noUsers: string
    inviteUser: string
}

export interface Props extends Page {
    usersList: Array<Member>
    contentText: ContentText
}

/**
 * Admin users dashboard template
 */
export const AdminUsersPage: (props: Props) => JSX.Element = ({
    contentText,
    actions,
    pagination,
    routes,
    usersList,
    user,
}) => {
    const [dynamicUsersList, setUsersList] = useState(usersList)
    const [dynamicPagination, setPagination] = useState(pagination)

    const { secondaryHeading, noUsers, inviteUser } = contentText ?? {}

    const hasUsers: boolean = true
    const shouldEnableLoadMore: boolean = true
    const shouldRenderCreateUserLink: boolean = actions.includes(
        actionConstants.SITE_ADMIN_MEMBERS_ADD
    )

    const columnList = [
        {
            children: 'Name',
            className: '',
        },
        {
            children: 'Role',
            className: '',
        },
        {
            children: 'Date joined',
            className: '',
        },
        {
            children: 'Last logged in',
            className: '',
        },
        {
            children: `Actions`,
            className: 'tablet:u-text-right',
        },
    ]

    const rowList = useMemo(
        () =>
            dynamicUsersList?.map(
                ({ id, fullName, role, joinDate, lastLogInDate }) => {
                    const generatedCellClasses = {
                        name: classNames({
                            ['u-justify-between u-w-full tablet:u-w-1/4 o-truncated-text-lines-1']:
                                true,
                        }),
                        role: classNames({
                            ['u-justify-between u-w-full tablet:u-w-1/4']: true,
                        }),
                        joinDate: classNames({
                            ['u-justify-between u-w-full tablet:u-w-1/6']: true,
                        }),
                        lastLoginDate: classNames({
                            ['u-justify-between u-w-full tablet:u-w-1/6']: true,
                        }),
                    }

                    const generatedHeaderCellClasses = {
                        name: classNames({
                            ['u-text-bold']: true,
                        }),
                        role: classNames({
                            ['u-text-bold']: true,
                        }),
                        joinDate: classNames({
                            ['u-text-bold']: true,
                        }),
                        lastLoginDate: classNames({
                            ['u-text-bold']: true,
                        }),
                    }

                    const rows = [
                        {
                            children: (
                                <Link href={`/users/${id}`}>{fullName}</Link>
                            ),
                            className: generatedCellClasses.name,
                            headerClassName: generatedHeaderCellClasses.name,
                        },
                        {
                            children: `${capitalise({ value: role })}`,
                            className: generatedCellClasses.role,
                            headerClassName: generatedHeaderCellClasses.role,
                            shouldRenderCellHeader: true,
                        },
                        {
                            children: `${dateTime({ value: joinDate })}`,
                            className: generatedCellClasses.joinDate,
                            headerClassName:
                                generatedHeaderCellClasses.joinDate,
                            shouldRenderCellHeader: true,
                        },
                        {
                            children: `${dateTime({ value: lastLogInDate })}`,
                            className: generatedCellClasses.lastLoginDate,
                            headerClassName:
                                generatedHeaderCellClasses.lastLoginDate,
                            shouldRenderCellHeader: true,
                        },
                        {
                            children: (
                                <ActionLink
                                    href={`${routes.usersRoot}/${id}/update`}
                                    text={{
                                        body: 'Edit',
                                        ariaLabel: `Edit user ${
                                            fullName || role
                                        }`,
                                    }}
                                    iconName="icon-edit"
                                />
                            ),
                            className:
                                'u-w-full tablet:u-w-1/8 tablet:u-text-right',
                            headerClassName: 'u-hidden',
                        },
                    ]

                    return rows
                }
            ),
        [dynamicUsersList]
    )

    /**
     * Handle client-side pagination
     */
    const handleGetPage = async ({
        pageNumber: requestedPageNumber,
        pageSize: requestedPageSize,
    }) => {
        const { data: additionalUsers, pagination } = await getSiteUsers({
            user: user,
            pagination: {
                pageNumber: requestedPageNumber,
                pageSize: requestedPageSize,
            },
        })

        setUsersList([...dynamicUsersList, ...additionalUsers])
        setPagination(pagination)
    }

    return (
        <>
            <LayoutColumnContainer className="u-w-full u-flex u-flex-col tablet:u-flex-row-reverse">
                {shouldRenderCreateUserLink && (
                    <LayoutColumn tablet={3} className="c-page-body">
                        <Link href={routes.adminUsersInvite}>
                            <a className="c-button u-w-full">{inviteUser}</a>
                        </Link>
                    </LayoutColumn>
                )}
                <LayoutColumn tablet={9} className="c-page-body">
                    <h2 className="nhsuk-heading-l">{secondaryHeading}</h2>
                    {hasUsers ? (
                        <DynamicListContainer
                            containerElementType="div"
                            shouldEnableLoadMore={shouldEnableLoadMore}
                            className="u-list-none u-p-0"
                        >
                            <DataGrid
                                id="admin-table-users"
                                columnList={columnList}
                                rowList={rowList}
                                text={{
                                    caption: 'Site users',
                                }}
                            />
                        </DynamicListContainer>
                    ) : (
                        <p>{noUsers}</p>
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
            routeId: '11fcd020-86e3-4935-982d-891bd86b52ff',
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
                const [usersList] = await Promise.all([
                    getSiteUsers({ user, pagination }),
                ])

                props.usersList = usersList.data ?? []
                props.pagination = usersList.pagination
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
export default AdminUsersPage
