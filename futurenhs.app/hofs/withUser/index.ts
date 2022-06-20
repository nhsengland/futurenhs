import { GetServerSideProps } from 'next'
import { getSession } from "next-auth/react"

import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { getSiteActions } from '@services/getSiteActions'
import { getUserInfo } from '@services/getUserInfo'
import { GetUserInfoService } from '@services/getUserInfo'
import { GetServerSidePropsContext, HofConfig } from '@appTypes/next'
import { getSiteUser } from '@services/getSiteUser'

export const withUser = (
    config: HofConfig,
    dependencies?: {
        getUserInfoService?: GetUserInfoService
        getSiteActionsService?: any
    }
): GetServerSideProps => {
    const getUserInfoService = dependencies?.getUserInfoService ?? getUserInfo
    const getSiteActionsService =
        dependencies?.getSiteActionsService ?? getSiteActions

    const { props, isRequired = true, getServerSideProps } = config

    return async (context: GetServerSidePropsContext): Promise<any> => {
        props.user = null

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
                    subjectId: (session.sub as string),
                    emailAddress: session.user?.email
                })
    
                props.user = user
                context.req.user = user

                if(isRequired){
    
                    if(user.status === 'LegacyMember' || user.status === 'Invited'){

                        const targetPath: string = `/users/${user.id}/create`;

                        if(context.resolvedUrl !== targetPath){

                            return {
                                redirect: {
                                    permanent: false,
                                    destination: `${process.env.APP_URL}${targetPath}`,
                                }, 
                            }

                        }

                    } else if(user.status === 'Uninvited'){

                        const targetPath: string = `/auth/unregistered`;

                        if(context.resolvedUrl !== targetPath){

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
                
                return handleSSRErrorProps({ props, error })

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
