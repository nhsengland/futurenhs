import { GetServerSideProps } from 'next';
import { GetServerSidePropsContext } from '@appTypes/next';
import { getAuth } from '@services/getAuth';
import { GetAuthService } from '@services/getAuth';

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
                    destination: process.env.NEXT_PUBLIC_LOGIN_URL
                }
            }    
    
        } else {

            context.req.user = data;

            return await getServerSideProps(context);

        }

    }

}
