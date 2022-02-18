import { GetServerSideProps } from 'next';
import { getAuth } from '@services/getAuth';
import { GetAuthService } from '@services/getAuth';
import { GetServerSidePropsContext, HofConfig } from '@appTypes/next';

export const withAuth = (config: HofConfig, dependencies?: {
    getAuthService?: GetAuthService
}): GetServerSideProps => {

    const getAuthService = dependencies?.getAuthService ?? getAuth;

    const { getServerSideProps } = config;

    return async (context: GetServerSidePropsContext): Promise<any> => {

        try {

            const { data } = await getAuthService({
                cookies: context.req.cookies
            });

            context.props = context.props || {};
            context.props.user = data;
            context.req.user = data;

            return await getServerSideProps(context);

        } catch (error) {

            const returnUrl: string = encodeURI(`${process.env.APP_URL}${context.resolvedUrl}`);

            return {
                redirect: {
                    permanent: false,
                    destination: `${process.env.NEXT_PUBLIC_MVC_FORUM_LOGIN_URL}?ReturnUrl=${returnUrl}`
                }
            }

        }

    }

}
