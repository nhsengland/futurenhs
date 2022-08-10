import { GetServerSideProps } from 'next'

import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { routeParams } from '@constants/routes'
import { layoutIds, groupTabIds } from '@constants/routes'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withGroup } from '@hofs/withGroup'
import { withTextContent } from '@hofs/withTextContent'
import { selectPageProps } from '@selectors/context'
import { getGroupFolderContents } from '@services/getGroupFolderContents'
import { selectUser, selectPagination, selectParam } from '@selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { Pagination } from '@appTypes/pagination'

import { GroupFolderContentsTemplate } from '@components/_pageTemplates/GroupFolderContentsTemplate'

/**
 * Get props to inject into page on the initial server-side request
 */
 export const getServerSideProps: GetServerSideProps = async (context: GetServerSidePropsContext) => await pipeSSRProps(context, {
    routeId: '8b74608e-e22d-4dd9-9501-1946ac27e133'
}, [
    withUser,
    withRoutes,
    withGroup,
    withTextContent
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
    const pagination: Pagination = {
        pageNumber: selectPagination(context).pageNumber ?? 1,
        pageSize: selectPagination(context).pageSize ?? 10,
    }

    props.layoutId = layoutIds.GROUP
    props.tabId = groupTabIds.FILES
    props.pageTitle = `${props.entityText.title} - ${props.contentText.subTitle}`

    /**
     * Get data from services
     */
    try {
        const [groupFolderContents] = await Promise.all([
            getGroupFolderContents({
                user,
                groupId,
                pagination,
            }),
        ])

        props.folderContents = groupFolderContents.data ?? []
        props.pagination = groupFolderContents.pagination
    } catch (error) {
        return handleSSRErrorProps({ props, error })
    }

    /**
     * Return data to page template
     */
    return handleSSRSuccessProps({ props, context })

});

/**
 * Export page template
 */
export default GroupFolderContentsTemplate
