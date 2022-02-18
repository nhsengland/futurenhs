import { GetServerSideProps } from 'next';
import { GetServerSidePropsContext, HofConfig } from '@appTypes/next';

export const withLogOut = (config: HofConfig, dependencies?: {}): GetServerSideProps => {

    const { getServerSideProps } = config;

    return async (context: GetServerSidePropsContext): Promise<any> => {

        for (var prop in context.req.cookies) {

            (context.res as any).cookie(prop, '', {
                ['expires']: new Date(0),
                ['max-age']: 0
            });
    
        }

        return await getServerSideProps(context);

    }

}