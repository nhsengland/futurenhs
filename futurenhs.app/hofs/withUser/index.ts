import { GetServerSideProps } from 'next';
import { getSiteActions } from '@services/getSiteActions';
import { getUser } from '@services/getUser';
import { GetUserService } from '@services/getUser';
import { GetServerSidePropsContext, HofConfig } from '@appTypes/next';

export const withUser = (config: HofConfig, dependencies?: {
    getAuthService?: GetUserService
}): GetServerSideProps => {

    const getAuthService = dependencies?.getAuthService ?? getUser;

    const { props, 
            isRequired = true, 
            getServerSideProps } = config;

    return async (context: GetServerSidePropsContext): Promise<any> => {

        try {

            const { data: user } = await getAuthService({
                cookies: context.req.cookies
            });

            props.user = user;
            context.req.user = user;

            const { data: actions } = await getSiteActions({ user });

            props.actions = actions;

            return await getServerSideProps(context);

        } catch (error) {

            props.user = null;

            if(isRequired){

                const returnUrl: string = encodeURI(`${process.env.APP_URL}${context.resolvedUrl}`);

                return {
                    redirect: {
                        permanent: false,
                        destination: `${process.env.NEXT_PUBLIC_MVC_FORUM_LOGIN_URL}?ReturnUrl=${returnUrl}`
                    }
                }

            }

            return await getServerSideProps(context);

        }

    }

}
