import { GetServerSideProps } from 'next'
import { getSession } from "next-auth/react"

import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { getSiteActions } from '@services/getSiteActions'
import { getUserInfo } from '@services/getUserInfo'
import { GetUserInfoService } from '@services/getUserInfo'
import { GetServerSidePropsContext, HofConfig } from '@appTypes/next'
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
): GetServerSideProps => {
    const getUserInfoService = dependencies?.getUserInfoService ?? getUserInfo
    const getSiteActionsService =
        dependencies?.getSiteActionsService ?? getSiteActions

    const isRequired: boolean = config?.isRequired ?? true;

    let user: User = null;

        const session = await getSession(context);

        if(!session && isRequired){

            return {
                redirect: {
                    permanent: false,
                    destination: `${process.env.APP_URL}/auth/signin`,
                }, 
            }

        }

        if(session){

            try {

                const { data: user } = await getUserInfoService({
                    identityId: (session.sub as string),
                    emailAddress: session.user?.email
                })
    
                props.user = user
                context.req.user = user
    
            } catch (error) {
        
                if(error.data?.status === 403 && isRequired){
    
                    return {
                        redirect: {
                            permanent: false,
                            destination: `${process.env.APP_URL}/auth/unregistered`,
                        }, 
                    }
    
                }
                
                return handleSSRErrorProps({ props, error })

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
