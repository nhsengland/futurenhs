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
import { getSession } from 'next-auth/react'
import { routes } from '@constants/routes'

const isMember: boolean = true

export interface Props extends Page {
    isGroupMember: boolean
    groupsList: Array<Group>
    B2CSignOut: string
}

/**
 * Group listing template
 */
export const GroupsPage: (props: Props) => JSX.Element = ({
    user,
    contentText,
    isGroupMember,
    groupsList,
    pagination,
    B2CSignOut,
}) => {
    console.log(B2CSignOut)
    const { pathname } = useRouter()

    const [dynamicGroupsList, setGroupsList] = useState(groupsList)
    const [dynamicPagination, setPagination] = useState(pagination)

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
        const { data: additionalGroups, pagination } = await getGroups({
            user: user,
            isMember: isGroupMember,
            pagination: {
                pageNumber: requestedPageNumber,
                pageSize: requestedPageSize,
            },
        })

        setGroupsList([...dynamicGroupsList, ...additionalGroups])
        setPagination(pagination)
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
                <a href={B2CSignOut}>SIGN OUT</a>
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
                    className="u-bg-theme-14"
                />
                <PageBody>
                    <LayoutColumn desktop={8}>
                        <h2 className="nhsuk-heading-l">{secondaryHeading}</h2>
                        {intro && (
                            <p className="u-text-lead u-text-theme-7 u-mb-4">
                                {intro}
                            </p>
                        )}
                        <DynamicListContainer
                            containerElementType="ul"
                            shouldEnableLoadMore={shouldEnableLoadMore}
                            className="u-list-none u-p-0"
                        >
                            {dynamicGroupsList?.map?.((teaserData, index) => {
                                return (
                                    <li key={index}>
                                        <GroupTeaser {...teaserData} />
                                    </li>
                                )
                            })}
                        </DynamicListContainer>
                        <PaginationWithStatus
                            id="group-list-pagination"
                            shouldEnableLoadMore={shouldEnableLoadMore}
                            getPageAction={handleGetPage}
                            {...dynamicPagination}
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
            const session = await getSession(context)
            const idTokenHint: string = session.id_token as string
            const signOutPath = routes.SIGN_OUT
            const callbackUrl: string = `${process.env.APP_URL}${signOutPath}`
            const B2CSignOutURL = `https://${process.env.AZURE_AD_B2C_TENANT_NAME}.b2clogin.com/${process.env.AZURE_AD_B2C_TENANT_NAME}.onmicrosoft.com/${process.env.AZURE_AD_B2C_PRIMARY_USER_FLOW}/oauth2/v2.0/logout?post_logout_redirect_uri=${callbackUrl}&id_token_hint=${idTokenHint}`

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
                const [groupsList] = await Promise.all([
                    getGroups({ user, isMember, pagination }),
                ])

                props.groupsList = groupsList.data ?? []
                props.pagination = groupsList.pagination
                props.B2CSignOut = B2CSignOutURL
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
