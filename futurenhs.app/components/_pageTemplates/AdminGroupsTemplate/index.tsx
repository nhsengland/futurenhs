import { useState, useMemo } from 'react'
import { useRouter } from 'next/router'
import classNames from 'classnames'

import { actions as actionConstants } from '@constants/actions'
import { Link } from '@components/Link'
import { ActionLink } from '@components/ActionLink'
import { PaginationWithStatus } from '@components/PaginationWithStatus'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { DynamicListContainer } from '@components/DynamicListContainer'
import { DataGrid } from '@components/DataGrid'

import { Props } from './interfaces'
import { getSiteGroups } from '@services/getSiteGroups'

/**
 * Admin groups dashboard template
 */
export const AdminGroupsTemplate: (props: Props) => JSX.Element = ({
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
