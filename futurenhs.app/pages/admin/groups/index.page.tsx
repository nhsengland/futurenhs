import { GetServerSideProps } from 'next'
import { useState, useMemo } from 'react'
import { useRouter } from 'next/router'
import classNames from 'classnames'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { layoutIds } from '@constants/routes'
import { actions } from '@constants/actions'
import { withUser } from '@hofs/withUser'
import { withTextContent } from '@hofs/withTextContent'
import { withRoutes } from '@hofs/withRoutes'
import { selectPageProps } from '@selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { selectUser, selectPagination } from '@selectors/context'
import { Pagination } from '@appTypes/pagination'
import { getSiteGroups } from '@services/getSiteGroups'
import { actions as actionConstants } from '@constants/actions'
import { Link } from '@components/Link'
import { ActionLink } from '@components/ActionLink'
import { PaginationWithStatus } from '@components/PaginationWithStatus'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { DynamicListContainer } from '@components/DynamicListContainer'
import { DataGrid } from '@components/DataGrid'
import { Group } from '@appTypes/group'
import { Page } from '@appTypes/page'
import { GenericPageTextContent } from '@appTypes/content'

declare interface ContentText extends GenericPageTextContent {
    usersHeading: string
    noGroups: string
    createGroup: string
}

export interface Props extends Page {
    groupsList: Array<Group>
    contentText: ContentText
}

/**
 * Admin groups dashboard template
 */
export const AdminGroupsPage: (props: Props) => JSX.Element = ({
    contentText,
    actions,
    pagination,
    groupsList,
    routes,
    user,
    services = {
        getSiteGroups: getSiteGroups,
    },
}) => {
    const router = useRouter()

    const [dynamicGroupsList, setGroupsList] = useState(groupsList)
    const [dynamicPagination, setPagination] = useState(pagination)

    const { secondaryHeading, noGroups, createGroup } = contentText ?? {}

    const hasUsers: boolean = true
    const shouldEnableLoadMore: boolean = true
    const shouldRenderCreateUserLink: boolean = actions.includes(
        actionConstants.SITE_ADMIN_MEMBERS_ADD
    )

    const columnList = [
        {
            children: 'Name',
            className: 'u-w-full tablet:u-w-2/5',
        },
        {
            children: 'Members',
            className: 'u-w-full tablet:u-w-1/5 tablet:u-text-center',
        },
        {
            children: 'Owner',
            className: 'u-w-full tablet:u-w-1/6',
        },
        {
            children: `Actions`,
            className: 'u-w-full tablet:u-w-1/6 tablet:u-text-right',
        },
    ]

    const rowList = useMemo(
        () =>
            dynamicGroupsList?.map(
                ({
                    text,
                    groupId,
                    owner,
                    totalDiscussionCount,
                    totalMemberCount,
                }) => {
                    const { mainHeading } = text ?? {}
                    const { id: ownerId, fullName } = owner ?? {}

                    const generatedCellClasses = {
                        name: classNames({
                            ['u-justify-between u-w-full']: true,
                        }),
                        members: classNames({
                            ['u-justify-between u-w-full tablet:u-text-center']:
                                true,
                        }),
                        owner: classNames({
                            ['u-justify-between u-w-full']: true,
                        }),
                        actions: classNames({
                            ['u-justify-between u-w-full']: true,
                        }),
                    }

                    const generatedHeaderCellClasses = {
                        name: classNames({
                            ['u-text-bold']: true,
                        }),
                        members: classNames({
                            ['u-text-bold']: true,
                        }),
                        owner: classNames({
                            ['u-text-bold']: true,
                        }),
                        actions: classNames({
                            ['u-text-bold']: true,
                        }),
                    }

                    const rows = [
                        {
                            children: (
                                <ActionLink
                                    href={`${routes.groupsRoot}/${groupId}`}
                                    className="o-truncated-text-lines-3"
                                    text={{
                                        body: mainHeading,
                                        ariaLabel: `Go to group ${mainHeading}`,
                                    }}
                                />
                            ),
                            className: generatedCellClasses.name,
                            headerClassName: generatedHeaderCellClasses.name,
                        },
                        {
                            children: (
                                <ActionLink
                                    href={`${routes.groupsRoot}/${groupId}/members`}
                                    text={{
                                        body: totalMemberCount?.toString(),
                                        ariaLabel: `Go to group ${mainHeading} members list`,
                                    }}
                                />
                            ),
                            className: generatedCellClasses.members,
                            headerClassName: generatedHeaderCellClasses.members,
                            shouldRenderCellHeader: true,
                        },
                        {
                            children: (
                                <ActionLink
                                    href={`${routes.usersRoot}/${ownerId}`}
                                    className="o-truncated-text-lines-3"
                                    text={{
                                        body: fullName,
                                        ariaLabel: `Go to group owner ${fullName} profile`,
                                    }}
                                />
                            ),
                            className: generatedCellClasses.owner,
                            headerClassName: generatedHeaderCellClasses.owner,
                            shouldRenderCellHeader: true,
                        },
                        {
                            children: (
                                <ActionLink
                                    href={`${routes.groupsRoot}/${groupId}/update`}
                                    text={{
                                        body: 'Edit',
                                        ariaLabel: `Edit group ${mainHeading}`,
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
        [dynamicGroupsList]
    )

    /**
     * Handle client-side pagination
     */
    const handleGetPage = async ({
        pageNumber: requestedPageNumber,
        pageSize: requestedPageSize,
    }) => {
        const { data: additionalGroups, pagination } =
            await services.getSiteGroups({
                user: user,
                pagination: {
                    pageNumber: requestedPageNumber,
                    pageSize: requestedPageSize,
                },
            })

        setGroupsList([...dynamicGroupsList, ...additionalGroups])
        setPagination(pagination)
    }

    return (
        <>
            <LayoutColumnContainer className="u-w-full u-flex u-flex-col-reverse tablet:u-flex-row">
                <LayoutColumn tablet={9} className="c-page-body">
                    <h2 className="nhsuk-heading-l">{secondaryHeading}</h2>
                    {hasUsers ? (
                        <DynamicListContainer
                            containerElementType="div"
                            shouldEnableLoadMore={shouldEnableLoadMore}
                            className="u-list-none u-p-0"
                        >
                            <DataGrid
                                id="admin-table-groups"
                                columnList={columnList}
                                rowList={rowList}
                                text={{
                                    caption: 'Site users',
                                }}
                            />
                        </DynamicListContainer>
                    ) : (
                        <p>{noGroups}</p>
                    )}
                    <PaginationWithStatus
                        id="group-list-pagination"
                        shouldEnableLoadMore={shouldEnableLoadMore}
                        getPageAction={handleGetPage}
                        {...dynamicPagination}
                    />
                </LayoutColumn>
                {shouldRenderCreateUserLink && (
                    <LayoutColumn tablet={3} className="c-page-body">
                        <Link href={`${router.asPath}/create`}>
                            <a className="c-button u-w-full">{createGroup}</a>
                        </Link>
                    </LayoutColumn>
                )}
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
            routeId: '5943d34d-ee73-46da-bb9a-917ba8a2f421',
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
                const [groupsList] = await Promise.all([
                    getSiteGroups({ user, pagination }),
                ])

                props.groupsList = groupsList.data ?? []
                props.pagination = groupsList.pagination
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
export default AdminGroupsPage
