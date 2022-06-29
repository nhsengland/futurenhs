import { GetServerSideProps } from 'next'

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

        console.log('withUser');

        props.user = null

        try {
            const { data: user } = await getUserService({
                cookies: context.req?.cookies,
            })

            props.user = user
            context.req.user = user

        } catch (error) {
            if (isRequired) {
                const returnUrl: string = encodeURI(
                    `${process.env.APP_URL}${context.resolvedUrl}`
                )

                return {
                    redirect: {
                        permanent: false,
                        destination: `${process.env.NEXT_PUBLIC_MVC_FORUM_LOGIN_URL}?ReturnUrl=${returnUrl}`,
                    },
                }
            }
        }

        /**
         * Temporary solution until new auth is in place to fetch users profile image from a separate endpoint
         */
        if (props.user) {

            console.log('withUser:props.user', props.user);

            try {
                const { data: profile } = await getSiteUser({
                    user: props.user,
                    targetUserId: props.user.id,
                })

                console.log('withUser:profile', profile);

                props.user.image = profile.image
            } catch (error) {
                console.log(error)
            }

            try {
                const { data: actions } = await getSiteActionsService({
                    user: props.user,
                })

                props.actions = actions

                console.log('withUser:actions', actions);

            } catch (error) {
                return handleSSRErrorProps({ props, error })
            }
        }

        return await getServerSideProps(context)
    }
}
