import { GetServerSideProps } from 'next'

import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { layoutIds, groupTabIds, routeParams } from '@constants/routes'
import { withUser } from '@hofs/withUser'
import { withGroup } from '@hofs/withGroup'
import { withRoutes } from '@hofs/withRoutes'
import { withTextContent } from '@hofs/withTextContent'
import { User } from '@appTypes/user'
import { selectUser, selectParam, selectPageProps } from '@selectors/context'
import { getGroupHomePageCmsContentIds } from '@services/getGroupHomePageCmsContentIds'
import { getCmsPageTemplate } from '@services/getCmsPageTemplate'
import { getCmsPageContent } from '@services/getCmsPageContent'
import { GetServerSidePropsContext } from '@appTypes/next'

import { GroupHomeTemplate } from '@components/_pageTemplates/GroupHomeTemplate'
import { Props } from '@components/_pageTemplates/GroupHomeTemplate/interfaces'

export const getServerSideProps: GetServerSideProps = async (context: GetServerSidePropsContext) => await pipeSSRProps(context, {
    routeId: '7a9bdd18-45ea-4976-9810-2fcb66242e27'
}, [
    withUser,
    withRoutes,
    withGroup,
    withTextContent
], async (context: GetServerSidePropsContext) => {

    const props: Partial<Props> = selectPageProps(context);
    const user: User = selectUser(context)
    const groupId: string = selectParam(
        context,
        routeParams.GROUPID
    )

    props.layoutId = layoutIds.GROUP
    props.tabId = groupTabIds.INDEX
    props.pageTitle = props.entityText?.title

    /**
     * Get data from services
     */
    try {

        const contentTemplateId: string =
            '0b955a4a-9e26-43e8-bb4b-51010e264d64'
        const groupHomePageCmsContentIds =
            await getGroupHomePageCmsContentIds({
                user,
                groupId,
            })
        const contentPageId: string =
            groupHomePageCmsContentIds.data.contentRootId

        const [contentBlocks, contentTemplate] =
            await Promise.all([
                getCmsPageContent({
                    user,
                    pageId: contentPageId,
                    isPublished: true,
                }),
                getCmsPageTemplate({
                    user,
                    templateId: contentTemplateId,
                }),
            ])

            ; (props.contentPageId = contentPageId),
                (props.contentTemplateId = contentTemplateId)

        props.contentBlocks = contentBlocks.data
        props.contentTemplate = contentTemplate.data
    } catch (error) {
        return handleSSRErrorProps({
            props,
            error,
            shouldSurface: false,
        })
    }

    /**
     * Return data to page template
     */
    return handleSSRSuccessProps({ props, context })

});

/**
 * Export page template
 */
export default GroupHomeTemplate
