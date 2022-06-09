import { GetServerSideProps } from 'next'
import { getSession } from "next-auth/react"

import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { getSiteActions } from '@services/getSiteActions'
import { getUser } from '@services/getUser'
import { GetUserService } from '@services/getUser'
import { GetServerSidePropsContext, HofConfig } from '@appTypes/next'
import { getSiteUser } from '@services/getSiteUser'

export const withUser = (
    config: HofConfig,
    dependencies?: {
        getUserService?: GetUserService
        getSiteActionsService?: any
    }
): GetServerSideProps => {
    const getUserService = dependencies?.getUserService ?? getUser
    const getSiteActionsService =
        dependencies?.getSiteActionsService ?? getSiteActions

    const { props, isRequired = true, getServerSideProps } = config

    return async (context: GetServerSidePropsContext): Promise<any> => {
        props.user = null

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

        /**
         * Temporary solution until new auth is in place to fetch users profile image from a separate endpoint
         */
        if (props.user) {
            try {
                const { data: profile } = await getSiteUser({
                    user: props.user,
                    targetUserId: props.user.id,
                })

                props.user.image = profile.image
            } catch (error) {
                console.log(error)
            }

            try {
                const { data: actions } = await getSiteActionsService({
                    user: props.user,
                })

                props.actions = actions
            } catch (error) {
                return handleSSRErrorProps({ props, error })
            }
        }

        return await getServerSideProps(context)
    }
}
