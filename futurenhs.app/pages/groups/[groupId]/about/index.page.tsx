import { GetServerSideProps } from 'next'

import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { layoutIds, groupTabIds } from '@constants/routes'
import { withUser } from '@hofs/withUser'
import { withGroup } from '@hofs/withGroup'
import { withRoutes } from '@hofs/withRoutes'
import { withTextContent } from '@hofs/withTextContent'
import { selectPageProps } from '@selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'

import { Props } from '@components/_pageTemplates/GroupHomeTemplate/interfaces'
import { GroupAboutUsTemplate } from '@components/_pageTemplates/GroupAboutUsTemplate'

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (context: GetServerSidePropsContext) => await pipeSSRProps(context, {
    routeId: 'cfe2a86c-f17f-4fbe-843f-7f43f5d7ad06'
}, [
    withUser,
    withRoutes,
    withGroup,
    withTextContent
], async (context: GetServerSidePropsContext) => {

    /**
     * Get data from request context
     */
    const props: Partial<Props> = selectPageProps(context);
    props.layoutId = layoutIds.GROUP
    props.tabId = groupTabIds.ABOUT
    props.pageTitle = `${props.entityText.title} - ${props.contentText.subTitle}`

    /**
     * Return data to page template
     */
    return handleSSRSuccessProps({ props, context })

});

/**
 * Export page template
 */
export default GroupAboutUsTemplate
