import { GetServerSideProps } from 'next'

import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { layoutIds, groupTabIds } from '@constants/routes'
import { routeParams } from '@constants/routes'
import { actions as actionConstants } from '@constants/actions'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withGroup } from '@hofs/withGroup'
import { deleteGroupFolder } from '@services/deleteGroupFolder'
import {
    selectParam,
    selectUser,
    selectQuery,
    selectCsrfToken,
    selectPageProps
} from '@selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'

import { GroupCreateUpdateFolderTemplate } from '@components/_pageTemplates/GroupCreateUpdateFolderTemplate'

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
    const user: User = selectUser(context)
    const groupId: string = selectParam(
        context,
        routeParams.GROUPID
    )
    const folderId: string = selectQuery(
        context,
        routeParams.FOLDERID
    )
    const csrfToken: string = selectCsrfToken(context)

    props.layoutId = layoutIds.GROUP
    props.tabId = groupTabIds.FILES
    props.folderId = folderId

    /**
     * Return not found if user does not have ability to delete folders in this group
     */
    if (
        !props.actions?.includes(
            actionConstants.GROUPS_FOLDERS_DELETE
        )
    ) {
        return {
            notFound: true,
        }
    }

    /**
     * Attempt to delete group folder
     */
    try {
        await deleteGroupFolder({
            user,
            groupId,
            folderId,
            csrfToken,
        })

        /**
         * Redirect to home
         */
        return {
            redirect: {
                permanent: false,
                destination: props.routes.groupFoldersRoot,
            },
        }
    } catch (error) {
        return handleSSRErrorProps({ error, props })
    }

});

/**
 * Export page template
 */
export default GroupCreateUpdateFolderTemplate
