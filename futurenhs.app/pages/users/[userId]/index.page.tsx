import { GetServerSideProps } from 'next'

import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { routeParams } from '@constants/routes'
import { withUser } from '@hofs/withUser'
import { withTextContent } from '@hofs/withTextContent'
import { withRoutes } from '@hofs/withRoutes'
import {
    selectParam,
    selectUser,
} from '@selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { getSiteUser } from '@services/getSiteUser'

import { SiteUserTemplate } from '@components/_pageTemplates/SiteUserTemplate'
import { Props } from '@components/_pageTemplates/SiteUserTemplate/interfaces'
import { User } from '@appTypes/user'

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
            getServerSideProps: async (
                context: GetServerSidePropsContext
            ) => {
                const userId: string = selectParam(
                    context,
                    routeParams.USERID
                )
                const user: User = selectUser(context)

                /**
                 * Get data from services
                 */
                try {
                    const [userData] = await Promise.all([
                        getSiteUser({ user, targetUserId: userId }),
                    ])

                    props.siteUser = userData.data
                    props.pageTitle = `${props.contentText.title} - ${props.siteUser.firstName ?? ''
                        } ${props.siteUser.lastName ?? ''}`
                } catch (error: any) {
                    return handleSSRErrorProps({ props, error })
                }

                /**
                 * Return data to page template
                 */
                return handleSSRSuccessProps({ props })
            },
        }),
    }),
})

/**
 * Export page template
 */
export default SiteUserTemplate
