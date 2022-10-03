import { defaultThemeId } from '@constants/themes'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { routeParams } from '@constants/routes'
import { defaultGroupLogos } from '@constants/icons'
import { selectUser, selectParam } from '@helpers/selectors/context'
import { getGroup } from '@services/getGroup'
import { GetGroupService } from '@services/getGroup'
import { getGroupActions } from '@services/getGroupActions'
import { GetGroupActionsService } from '@services/getGroupActions'
import { actions } from '@constants/actions'
import { User } from '@appTypes/user'
import { Hof } from '@appTypes/hof'

export const withGroup: Hof = async (
    context,
    config,
    dependencies?: {
        getGroupService?: GetGroupService
        getGroupActionsService?: GetGroupActionsService
    }
) => {
    const getGroupService = dependencies?.getGroupService ?? getGroup
    const getGroupActionsService =
        dependencies?.getGroupActionsService ?? getGroupActions

    /**
     * Get data from request context
     */
    const groupId: string = selectParam(context, routeParams.GROUPID)
    const user: User = selectUser(context)

    /**
     * Get data from services
     */
    try {
        const [groupData, actionsData] = await Promise.all([
            getGroupService({ user, groupId }),
            getGroupActionsService({ groupId, user }),
        ])

        context.page.props.groupId = groupId
        context.page.props.isPublic = groupData.data.isPublic
        context.page.props.themeId = groupData.data.themeId ?? defaultThemeId
        context.page.props.entityText = groupData.data.text ?? {}
        context.page.props.image =
            groupData.data.image ?? defaultGroupLogos.small
        context.page.props.memberStatus = actionsData.data.memberStatus
        context.page.props.actions = [
            ...(context.page.props.actions ?? []),
            ...(actionsData.data?.actions ?? []),
        ]

        const openRoutes: Array<string> = [
            context.page.props.routes.groupAboutRoot,
            context.page.props.routes.groupJoin,
        ]

        const hasAccess: boolean = actionsData.data?.actions?.includes(
            actions.GROUPS_VIEW
        )
        const isOpenRoute: boolean = openRoutes.some((route) =>
            context.resolvedUrl?.startsWith(route)
        )

        if (!context.page.props.isPublic) {
            if (!hasAccess && !isOpenRoute) {
                return {
                    props: context.page.props,
                    redirect: {
                        permanent: false,
                        destination: context.page.props.routes.groupAboutRoot,
                    },
                }
            }
        }
    } catch (error) {
        return handleSSRErrorProps({ props: context.page.props, error })
    }
}
