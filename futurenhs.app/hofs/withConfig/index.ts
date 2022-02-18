import { GetServerSideProps } from 'next';

import { routeParams } from '@constants/routes';
import { selectParam, selectProps } from '@selectors/context';
import { GetServerSidePropsContext, HofConfig } from '@appTypes/next';

export const withConfig = (config: HofConfig, dependencies?: {}): GetServerSideProps => {

    const { routeId, getServerSideProps } = config;

    return async (context: GetServerSidePropsContext): Promise<any> => {

        /**
         * TODO
         */
        return await getServerSideProps(context);

    }

}
