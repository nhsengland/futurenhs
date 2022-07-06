import { GetServerSideProps } from 'next'

import { actions as actionConstants } from '@constants/actions'
import { routeParams, layoutIds, groupTabIds } from '@constants/routes'
import { selectParam, selectCsrfToken, selectRequestMethod } from '@selectors/context'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withGroup } from '@hofs/withGroup'
import { requestMethods } from '@constants/fetch'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { deleteGroupMembership } from '@services/deleteGroupMembership'
import { GetServerSidePropsContext } from '@appTypes/next'

import { GroupUpdateTemplate } from '@components/_pageTemplates/GroupUpdateTemplate'
import { Props } from '@components/_pageTemplates/GroupUpdateTemplate/interfaces'

const props: Partial<Props> = {}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withUser({
    props,
    getServerSideProps: withRoutes({
        props,
        getServerSideProps: withGroup({
            props,
            getServerSideProps: async (context: GetServerSidePropsContext) => {

                const requestMethod: requestMethods =
                selectRequestMethod(context);
                const csrfToken: string = selectCsrfToken(context)
                const groupId: string = selectParam(
                    context,
                    routeParams.GROUPID
                )

                props.layoutId = layoutIds.GROUP
                props.tabId = groupTabIds.INDEX

                /**
                 * Return not found if user does not have valid action to leave group
                 */
                if (!props.actions?.includes(actionConstants.GROUPS_LEAVE)) {
                    return {
                        notFound: true,
                    }
                }

                /**
                 * Return error if request is not a POST
                 */
                if (requestMethod !== requestMethods.POST) {

                    return handleSSRErrorProps({ props, error: new Error('405 Method Not Allowed') })

                }

                /**
                 * Get data from services
                 */
                try {
                    await deleteGroupMembership({
                        groupId,
                        csrfToken,
                        user: props.user,
                    })

                    /**
                     * Return data to page template
                     */
                    return {
                        redirect: {
                            permanent: false,
                            destination: props.routes.groupRoot,
                        },
                    }
                } catch (error) {
                    return handleSSRErrorProps({ props, error })
                }
            },
        }),
    }),
})

/**
 * Export page template
 */
export default GroupUpdateTemplate
