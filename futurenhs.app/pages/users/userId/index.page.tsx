import { GetServerSideProps } from 'next'

import { withUser } from '@hofs/withUser'
import { withTextContent } from '@hofs/withTextContent'
import { withRoutes } from '@hofs/withRoutes'
import { GetServerSidePropsContext } from '@appTypes/next'

import { SiteUserTemplate } from '@components/_pageTemplates/SiteUserTemplate'
import { Props } from '@components/_pageTemplates/AdminUsersTemplate/interfaces'

const routeId: string = '9e86c5cc-6836-4319-8d9d-b96249d4c909'
const props: Partial<Props> = {}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withUser({
    props,
    getServerSideProps: withRoutes({
        props,
        getServerSideProps: withTextContent({
            props,
            routeId,
            getServerSideProps: async (context: GetServerSidePropsContext) => {
                /**
                 * Return data to page template
                 */
                return {
                    props: props,
                }
            },
        }),
    }),
})

/**
 * Export page template
 */
export default SiteUserTemplate
