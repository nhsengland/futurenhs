import { GetServerSideProps } from 'next'

import { defaultThemeId } from '@constants/themes'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { routeParams } from '@constants/routes'
import { defaultGroupLogos } from '@constants/icons'
import { selectUser, selectParam } from '@selectors/context'
import { getGroup } from '@services/getGroup'
import { GetGroupService } from '@services/getGroup'
import { getGroupActions } from '@services/getGroupActions'
import { GetGroupActionsService } from '@services/getGroupActions'
import { GetServerSidePropsContext, HofConfig } from '@appTypes/next'
import { User } from '@appTypes/user'

export const withGroup = (
    config: HofConfig,
    dependencies?: {
        getGroupService?: GetGroupService
        getGroupActionsService?: GetGroupActionsService
    }
): GetServerSideProps => {
    const getGroupService = dependencies?.getGroupService ?? getGroup
    const getGroupActionsService =
        dependencies?.getGroupActionsService ?? getGroupActions

    const { props, getServerSideProps } = config

    return async (context: GetServerSidePropsContext): Promise<any> => {
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

            props.groupId = groupId
            props.themeId = groupData.data.themeId ?? defaultThemeId
            props.entityText = groupData.data.text ?? {}
            props.image = groupData.data.image ?? defaultGroupLogos.small
            props.actions = [
                ...(props.actions ?? []),
                ...(actionsData.data ?? []),
            ]
        } catch (error) {
            return handleSSRErrorProps({ props, error })
        }

        return await getServerSideProps(context)
    }
}
