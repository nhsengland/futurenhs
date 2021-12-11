import { GetServerSideProps } from 'next';
import { getAuth } from '@services/getAuth';
import { GetAuthService } from '@services/getAuth';
import { getEnvVar } from '@helpers/util/env';
import { GetServerSidePropsContext } from '@appTypes/next';

export const withAuth = (getServerSideProps: GetServerSideProps, dependencies?: {
    getAuthService?: GetAuthService
}): GetServerSideProps => {

    const getAuthService = dependencies?.getAuthService ?? getAuth;

    return async (context: GetServerSidePropsContext): Promise<any> => {

        const { data, errors } = await getAuthService({
            cookies: context.req.cookies
        });

        if(!data || errors){

            return {
                redirect: {
                    permanent: false,
                    destination: getEnvVar({ name: 'NEXT_PUBLIC_MVC_FORUM_LOGIN_URL' })
                }
            }    
    
        } else {

            context.req.user = data;

            return await getServerSideProps(context);

        }

    }

}