import { GetServerSideProps } from 'next';

import { authCookie } from '@constants/cookies';
import { GetServerSidePropsContext, HofConfig } from '@appTypes/next';

export const withLogOut = (config: HofConfig, dependencies?: {}): GetServerSideProps => {

    const { getServerSideProps } = config;

    return async (context: GetServerSidePropsContext): Promise<any> => {

        for (var cookieName in context.req.cookies) {

            if(cookieName === authCookie){

                (context.res as any).cookie(cookieName, '', {
                    ['expires']: new Date(0),
                    ['max-age']: 0
                });

            }
    
        }

        return await getServerSideProps(context);

    }

}