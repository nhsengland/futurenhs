import { GetServerSideProps } from 'next'

import { selectCsrfToken } from '@selectors/context'
import { GetServerSidePropsContext, HofConfig } from '@appTypes/next'

export const withTokens = (
    config: HofConfig,
    dependencies?: {}
): GetServerSideProps => {
    const { props, getServerSideProps } = config

    return async (context: GetServerSidePropsContext): Promise<any> => {
        const csrfToken: string = selectCsrfToken(context)

        try {

            props.csrfToken = csrfToken

            return await getServerSideProps(context)
        } catch (error) {
            console.log(error)
        }
    }
}
