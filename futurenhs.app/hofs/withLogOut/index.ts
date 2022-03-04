import { GetServerSideProps } from 'next';

import { cookiePreferences } from '@constants/cookies';
import { GetServerSidePropsContext, HofConfig } from '@appTypes/next';

export const withLogOut = (config: HofConfig, dependencies?: {}): GetServerSideProps => {

    const { getServerSideProps } = config;

    const cookiesToRetain: Array<string> = [cookiePreferences.COOKIE_NAME];

    return async (context: GetServerSidePropsContext): Promise<any> => {

        for (var cookieName in context.req.cookies) {

            if(!cookiesToRetain.includes(cookieName)){

                (context.res as any).cookie(cookieName, '', {
                    ['expires']: new Date(0),
                    ['max-age']: 0
                });

            }
    
        }

        return await getServerSideProps(context);

    }

}