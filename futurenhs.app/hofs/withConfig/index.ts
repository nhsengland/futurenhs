import { GetServerSideProps } from 'next';

import { GetServerSidePropsContext, HofConfig } from '@appTypes/next';

export const withConfig = (config: HofConfig, dependencies?: {}): GetServerSideProps => {

    const { props, routeId, getServerSideProps } = config;

    return async (context: GetServerSidePropsContext): Promise<any> => {

        /**
         * TODO
         */
        return await getServerSideProps(context);

    }

}
