import { GetServerSideProps } from 'next'

import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { layoutIds, groupTabIds } from '@constants/routes'
import { routeParams } from '@constants/routes'
import { selectUser, selectParam, selectCsrfToken, selectPageProps } from '@selectors/context'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withGroup } from '@hofs/withGroup'
import { withTextContent } from '@hofs/withTextContent'
import { getGroupFile } from '@services/getGroupFile'
import { getGroupFileView } from '@services/getGroupFileView'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'

import { GroupFilePreviewTemplate } from '@components/_pageTemplates/GroupFilePreviewTemplate'

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (context: GetServerSidePropsContext) => await pipeSSRProps(context, {
    routeId: 'b74b9b6b-0462-4c2a-8859-51d0df17f68f'
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
    const csrfToken: string = selectCsrfToken(context)
    const user: User = selectUser(context)
    const groupId: string = selectParam(
        context,
        routeParams.GROUPID
    )
    const fileId: string = selectParam(
        context,
        routeParams.FILEID
    )

    props.fileId = fileId
    props.layoutId = layoutIds.GROUP
    props.tabId = groupTabIds.FILES
    props.shouldRenderPhaseBanner = false
    props.shouldRenderBreadCrumb = false
    props.shouldRenderGroupHeader = false

    /**
     * Get data from services
     */
    try {
        const [groupFile, groupFileView] = await Promise.all([
            getGroupFile({ user, groupId, fileId }),
            getGroupFileView({
                user,
                groupId,
                fileId,
                cookies: context.req?.cookies,
            }),
        ])

        props.csrfToken = csrfToken
        props.file = groupFile.data
        props.preview = groupFileView.data
        props.pageTitle = `${props.entityText.title} - ${props.file.name}`
    } catch (error) {
        console.log(error)

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
export default GroupFilePreviewTemplate
