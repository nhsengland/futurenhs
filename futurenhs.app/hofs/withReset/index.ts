import { GetServerSideProps } from 'next'

import { GetServerSidePropsContext, HofConfig } from '@appTypes/next'

export const withReset = (
    config: HofConfig,
    dependencies?: {}
): GetServerSideProps => {
    const { props, getServerSideProps } = config

    for(const key in props){

        delete props[key];

    }

    return async (context: GetServerSidePropsContext): Promise<any> => {
        return await getServerSideProps(context)
    }
}
