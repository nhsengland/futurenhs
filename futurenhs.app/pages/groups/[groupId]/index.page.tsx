import { GetServerSideProps } from 'next'

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { layoutIds, groupTabIds } from '@constants/routes'
import { withUser } from '@hofs/withUser'
import { withGroup } from '@hofs/withGroup'
import { withRoutes } from '@hofs/withRoutes'
import { withTextContent } from '@hofs/withTextContent'
import { GetServerSidePropsContext } from '@appTypes/next'

import { GroupHomeTemplate } from '@components/_pageTemplates/GroupHomeTemplate'
import { Props } from '@components/_pageTemplates/GroupHomeTemplate/interfaces'

const routeId: string = '7a9bdd18-45ea-4976-9810-2fcb66242e27'
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
            routeId,
            getServerSideProps: withTextContent({
                props,
                routeId,
                getServerSideProps: async (
                    context: GetServerSidePropsContext
                ) => {
                    props.layoutId = layoutIds.GROUP
                    props.tabId = groupTabIds.INDEX
                    props.pageTitle = props.entityText.title

                    /**
                     * Return data to page template
                     */
                    return handleSSRSuccessProps({ props })
                },
            }),
        }),
    }),
})

/**
 * Export page template
 */
export default GroupHomeTemplate
