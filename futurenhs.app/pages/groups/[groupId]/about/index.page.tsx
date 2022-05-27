import { GetServerSideProps } from 'next'

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { layoutIds, groupTabIds, routeParams } from '@constants/routes'
import { withUser } from '@hofs/withUser'
import { withGroup } from '@hofs/withGroup'
import { withRoutes } from '@hofs/withRoutes'
import { withTextContent } from '@hofs/withTextContent'
import { User } from '@appTypes/user'
import { GetServerSidePropsContext } from '@appTypes/next'

import { Props } from '@components/_pageTemplates/GroupHomeTemplate/interfaces'
import { GroupAboutUsTemplate } from '@components/_pageTemplates/GroupAboutUsTemplate'

const routeId: string = 'cfe2a86c-f17f-4fbe-843f-7f43f5d7ad06'
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
                    props.tabId = groupTabIds.ABOUT
                    props.pageTitle = `${props.entityText.title} - ${props.contentText.subTitle}`

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
export default GroupAboutUsTemplate
