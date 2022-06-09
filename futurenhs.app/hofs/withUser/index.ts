import { GetServerSideProps } from 'next'
import { getSession } from "next-auth/react"

import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { getSiteActions } from '@services/getSiteActions'
import { getUser } from '@services/getUser'
import { GetUserService } from '@services/getUser'
import { getSiteUser } from '@services/getSiteUser'
import { User } from '@appTypes/user'
import { Hof } from '@appTypes/hof'

export const withUser: Hof = async (
    context,
    config,
    dependencies?: {
        getUserService?: GetUserService
        getSiteUserService?: any;
        getSiteActionsService?: any;
    }
) => {

    const getUserService = dependencies?.getUserService ?? getUser
    const getSiteUserService = dependencies?.getSiteUserService ?? getSiteUser
    const getSiteActionsService =
        dependencies?.getSiteActionsService ?? getSiteActions

    const isRequired: boolean = config?.isRequired ?? true;

    let user: User = null;

        try {

            const session = await getSession(context);

            if(!session){

                throw new Error('No session')

            }

            console.log('session', session)

            // const { data: user } = await getUserService({
            //     cookies: context.req?.cookies,
            // })

            // props.user = user
            // context.req.user = user

        } catch (error) {
            if (isRequired) {

                return {
                    redirect: {
                        permanent: false,
                        destination: `${process.env.APP_URL}/auth/signin`,
                    }, 
                }

            }
        }
    }

    /**
     * Temporary solution until new auth is in place to fetch users profile image from a separate endpoint
     */
    if (user) {

        try {
            const { data: profile } = await getSiteUserService({ user, targetUserId: user.id })

            user.image = profile.image
        } catch (error) {
            return handleSSRErrorProps({ props: context.page.props, error })
        }

        try {
            const { data: actions } = await getSiteActionsService({ user })

            context.page.props.actions = actions

        } catch (error) {
            return handleSSRErrorProps({ props: context.page.props, error })
        }
    }

    context.req.user = user;
    context.page.props.user = user;

}
