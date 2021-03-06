import { GetServerSideProps } from 'next'

import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { actions as actionConstants } from '@constants/actions'
import { routeParams } from '@constants/routes'
import { layoutIds, groupTabIds } from '@constants/routes'
import { selectParam, selectCsrfToken, selectRequestMethod } from '@selectors/context'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withGroup } from '@hofs/withGroup'
import { selectPageProps } from '@selectors/context'
import { requestMethods } from '@constants/fetch'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { postGroupMembership } from '@services/postGroupMembership'
import { GetServerSidePropsContext } from '@appTypes/next'

import { GroupUpdateTemplate } from '@components/_pageTemplates/GroupUpdateTemplate'

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (context: GetServerSidePropsContext) => await pipeSSRProps(context, {}, [
    withUser,
    withRoutes,
    withGroup
], async (context: GetServerSidePropsContext) => {

    /**
     * Get data from request context
     */
    const props: Partial<any> = selectPageProps(context);
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
     * Return not found if user does not have valid action to join group
     */
    if (!props.actions?.includes(actionConstants.GROUPS_JOIN)) {
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
        await postGroupMembership({
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
});

/**
 * Export page template
 */
export default GroupUpdateTemplate
