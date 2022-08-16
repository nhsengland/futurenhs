import { getSession } from 'next-auth/react'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { getSiteActions } from '@services/getSiteActions'
import { getUserInfo } from '@services/getUserInfo'
import { GetUserInfoService } from '@services/getUserInfo'
import { getSiteUser } from '@services/getSiteUser'
import { User } from '@appTypes/user'
import { Hof } from '@appTypes/hof'

export const withUser: Hof = async (
    context,
    config,
    dependencies?: {
        getUserInfoService?: GetUserInfoService
        getSiteActionsService?: any
    }
) => {
    const getUserInfoService = dependencies?.getUserInfoService ?? getUserInfo
    const getSiteActionsService =
        dependencies?.getSiteActionsService ?? getSiteActions

    const isRequired: boolean = config?.isRequired ?? true

    let user: User = null

    const session = await getSession(context)

    if (!session && isRequired) {
        return {
            redirect: {
                permanent: false,
                destination: `${process.env.APP_URL}/auth/signin`,
            },
        }
    }

    if (session) {
        try {
            const { data } = await getUserInfoService({
                subjectId: session.sub as string,
                emailAddress: session.user?.email,
            })

            user = data
            context.req.user = user

            if (isRequired) {
                const { status } = user

                if (status === 'LegacyMember' || status === 'Invited') {
                    const targetPath: string = `/users/${user.id}/create`

                    if (context.resolvedUrl !== targetPath) {
                        return {
                            redirect: {
                                permanent: false,
                                destination: `${process.env.APP_URL}${targetPath}`,
                            },
                        }
                    }
                } else if (status === 'Uninvited') {
                    const targetPath: string = `/auth/unregistered`

                    if (context.resolvedUrl !== targetPath) {
                        return {
                            redirect: {
                                permanent: false,
                                destination: `${process.env.APP_URL}${targetPath}`,
                            },
                        }
                    }
                }
            }
        } catch (error) {
            return handleSSRErrorProps({ props: context.page.props, error })
        }
    }

    /**
     * Temporary solution until new auth is in place to fetch users profile image from a separate endpoint
     */
    if (user) {
        try {
            const { data: profile } = await getSiteUser({
                user: user,
                targetUserId: user.id,
            })

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

    context.req.user = user
    context.page.props.user = user
}
