import { GetServerSideProps } from 'next'

import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { withUser } from '@hofs/withUser'
import { withTextContent } from '@hofs/withTextContent'
import { withRoutes } from '@hofs/withRoutes'
import { selectPageProps } from '@selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'

import { GenericContentTemplate } from '@components/_pageTemplates/GenericContentTemplate'
import { Props } from '@components/_pageTemplates/GenericContentTemplate/interfaces'

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (context: GetServerSidePropsContext) => await pipeSSRProps(context, {
    routeId: '4dbd3bf6-969a-46b0-b38d-31cddfa61977'
}, [
    [withUser, { isRequired: false }],
    withRoutes,
    withTextContent
], async (context: GetServerSidePropsContext) => {

    /**
     * Get data from request context
     */
    const props: Partial<Props> = selectPageProps(context);

    /**
     * Return data to page template
     */
    return handleSSRSuccessProps({ props, context })

});

/**
 * Export page template
 */
export default GenericContentTemplate
