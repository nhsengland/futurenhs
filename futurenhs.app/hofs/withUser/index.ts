import { GetServerSideProps } from 'next'

import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { getSiteActions } from '@services/getSiteActions'
import { getUser } from '@services/getUser'
import { GetUserService } from '@services/getUser'
import { GetServerSidePropsContext, HofConfig } from '@appTypes/next'

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

        if (props.user) {
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
