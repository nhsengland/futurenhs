import { GetServerSideProps } from 'next'

import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { getRouteToParam2 } from '@helpers/routing/getRouteToParam'
import { GetServerSidePropsContext, HofConfig } from '@appTypes/next'

export const withRoutes = (
    config: HofConfig,
    dependencies?: {}
): GetServerSideProps => {
    const { props, getServerSideProps } = config

    return async (context: GetServerSidePropsContext): Promise<any> => {
        /**
         * Set up current routing data relative to context
         */
        try {
            const groupIndexRoute: string = getRouteToParam2({
                route: context.resolvedUrl,
                param: context.params?.groupId,
            })

            props.routes = getJsonSafeObject({
                object: {
                    siteRoot: '/',
                    adminRoot: '/admin',
                    adminUsersRoot: '/admin/users',
                    adminUsersInvite: '/admin/users/invite',
                    adminGroupsRoot: '/admin/groups',
                    adminGroupsCreate: '/admin/users/create',
                    groupsRoot: '/groups',
                    groupsDiscover: '/groups/discover',
                    groupRoot: groupIndexRoute ? groupIndexRoute : null,
                    groupUpdate: groupIndexRoute
                        ? `${groupIndexRoute}/update`
                        : null,
                    groupJoin: groupIndexRoute
                        ? `${groupIndexRoute}/join`
                        : null,
                    groupLeave: groupIndexRoute
                        ? `${groupIndexRoute}/leave`
                        : null,
                    groupForumRoot: groupIndexRoute
                        ? `${groupIndexRoute}/forum`
                        : null,
                    groupFoldersRoot: groupIndexRoute
                        ? `${groupIndexRoute}/folders`
                        : null,
                    groupFolder:
                        groupIndexRoute && context.params.folderId
                            ? `${groupIndexRoute}/folders/${context.params.folderId}`
                            : null,
                    groupFilesRoot: groupIndexRoute
                        ? `${groupIndexRoute}/files`
                        : null,
                    groupMembersRoot: groupIndexRoute
                        ? `${groupIndexRoute}/members`
                        : null,
                },
            })
        } catch (error) {
            return handleSSRErrorProps({ props, error })
        }

        return await getServerSideProps(context)
    }
}
