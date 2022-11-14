import { GetServerSideProps } from 'next'
import { useState } from 'react'
import Head from 'next/head'
import { useRouter } from 'next/router'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { withUser } from '@helpers/hofs/withUser'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { getGroups } from '@services/getGroups'
import {
    selectUser,
    selectPagination,
    selectPageProps,
} from '@helpers/selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { Pagination } from '@appTypes/pagination'
import { DynamicListContainer } from '@components/layouts/DynamicListContainer'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { GroupPageHeader } from '@components/blocks/GroupPageHeader'
import { GroupTeaser } from '@components/blocks/GroupTeaser'
import { PageBody } from '@components/layouts/PageBody'
import { PaginationWithStatus } from '@components/generic/PaginationWithStatus'
import { Page } from '@appTypes/page'
import { Group } from '@appTypes/group'
import { getGroupInvites } from '@services/getGroupInvites'

const isMember: boolean = true

export interface Props extends Page {
    isGroupMember: boolean
    groupsList: Array<Group>
    pendingList: Array<Group>
    groupsPagination: Pagination
    pendingPagination: Pagination
}

/**
 * Group listing template
 */
export const GroupsPage: (props: Props) => JSX.Element = ({
    user,
    contentText,
    isGroupMember,
    groupsList,
    pendingList,
    groupsPagination,
    pendingPagination,
}) => {
    const { pathname } = useRouter()

    const [dynamicGroupsList, setGroupsList] = useState(groupsList)
    const [dynamicPendingList, setPendingList] = useState(pendingList)

    const [dynamicGroupPagination, setGroupPagination] =
        useState(groupsPagination)
    const [dynamicPendingPagination, setPendingPagination] =
        useState(pendingPagination)

    const shouldEnableLoadMore: boolean = true
    const {
        title,
        metaDescription,
        mainHeading,
        intro,
        secondaryHeading,
        navMenuTitle,
    } = contentText ?? {}
    /**
     * Handle client-side pagination
     */
    const handleGetPage = async ({
        pageNumber: requestedPageNumber,
        pageSize: requestedPageSize,
    }) => {
        const [groupsRes, pendingRes] = await Promise.all([
            getGroups({
                user: user,
                isMember: isGroupMember,
                pagination: {
                    pageNumber: requestedPageNumber,
                    pageSize: requestedPageSize,
                },
            }),
            getGroupInvites({
                user: user,
                pagination: {
                    pageNumber: requestedPageNumber,
                    pageSize: requestedPageSize,
                },
            }),
        ])

        const { data: additionalGroups, pagination: groupsPagination } =
            groupsRes
        const { data: additionalPending, pagination: pendingPagination } =
            pendingRes

        setGroupsList([...dynamicGroupsList, ...additionalGroups])
        setPendingList([...dynamicPendingList, ...additionalPending])

        setGroupPagination(groupsPagination)
        setPendingPagination(pendingPagination)
    }

    const refreshGroupInvites = async () => {
        const { data, pagination } = await getGroupInvites({
            user: user,
            pagination: {
                pageNumber: dynamicPendingPagination.pageNumber,
                pageSize: dynamicPendingPagination.pageSize,
            },
        })

        setPendingList(data)
        setPendingPagination(pagination)
    }

    /**
     * Render
     */
    return (
        <>
            <Head>
                <title>{title}</title>
                <meta name="description" content={metaDescription} />
            </Head>
            <LayoutColumnContainer>
                <GroupPageHeader
                    id="my-groups"
                    text={{
                        mainHeading: mainHeading,
                        description: intro,
                        navMenuTitle: navMenuTitle,
                    }}
                    navMenuList={[
                        {
                            url: '/groups',
                            text: 'My groups',
                            isActive: pathname === '/groups',
                        },
                        {
                            url: '/groups/discover',
                            text: 'Discover new groups',
                            isActive: pathname === '/groups/discover',
                        },
                    ]}
                    shouldRenderActionsMenu={false}
                    isDiscover
                    className="u-bg-theme-14"
                />
                <PageBody>
                    <LayoutColumn desktop={8}>
                        <h2 className="nhsuk-heading-l">All my groups</h2>
                        {intro && (
                            <p className="u-text-lead u-text-theme-7 u-mb-4">
                                {intro}
                            </p>
                        )}
                    </LayoutColumn>
                    {!!dynamicPendingList.length && (
                        <LayoutColumn desktop={8} className="u-mb-14">
                            <h2 className="nhsuk-heading-m">
                                Groups you have been invited to
                            </h2>
                            <DynamicListContainer
                                containerElementType="ul"
                                shouldEnableLoadMore={shouldEnableLoadMore}
                                className="u-list-none u-p-0"
                            >
                                {dynamicPendingList?.map?.(
                                    (teaserData, index) => {
                                        return (
                                            <li key={index}>
                                                <GroupTeaser
                                                    {...teaserData}
                                                    groupInvite={
                                                        teaserData.invite
                                                    }
                                                    refreshGroupInvites={
                                                        refreshGroupInvites
                                                    }
                                                    user={user}
                                                    isPending
                                                />
                                            </li>
                                        )
                                    }
                                )}
                            </DynamicListContainer>
                            <PaginationWithStatus
                                id="group-list-pagination"
                                shouldEnableLoadMore={shouldEnableLoadMore}
                                getPageAction={handleGetPage}
                                {...dynamicPendingPagination}
                            />
                        </LayoutColumn>
                    )}
                    <LayoutColumn desktop={8}>
                        <h2 className="nhsuk-heading-m">
                            Groups you have joined
                        </h2>
                        <DynamicListContainer
                            containerElementType="ul"
                            shouldEnableLoadMore={shouldEnableLoadMore}
                            className="u-list-none u-p-0"
                        >
                            {dynamicGroupsList.length
                                ? dynamicGroupsList.map((teaserData, index) => {
                                      return (
                                          <li key={index}>
                                              <GroupTeaser {...teaserData} />
                                          </li>
                                      )
                                  })
                                : "You haven't joined any groups yet"}
                        </DynamicListContainer>
                        <PaginationWithStatus
                            id="group-list-pagination"
                            shouldEnableLoadMore={shouldEnableLoadMore}
                            getPageAction={handleGetPage}
                            {...dynamicGroupPagination}
                        />
                    </LayoutColumn>
                </PageBody>
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
            routeId: '3c745d1d-9742-459a-a2bb-7af14c2f291c',
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
                pageSize: selectPagination(context).pageSize ?? 10,
            }

            props.isGroupMember = isMember

            /**
             * Get data from services
             */
            try {
                const [groupsRes, pendingRes] = await Promise.all([
                    getGroups({ user, isMember, pagination }),
                    getGroupInvites({ user, pagination }),
                ])

                props.groupsList = groupsRes.data ?? []
                props.groupsPagination = groupsRes.pagination

                props.pendingList = pendingRes.data ?? []
                props.pendingPagination = pendingRes.pagination
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
export default GroupsPage
