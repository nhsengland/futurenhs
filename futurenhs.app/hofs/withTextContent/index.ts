import { GetServerSideProps } from 'next'

import { selectLocale } from '@selectors/context'
import { getPageTextContent } from '@services/getPageTextContent'
import { GetPageTextContentService } from '@services/getPageTextContent'
import { GetServerSidePropsContext, HofConfig } from '@appTypes/next'

export const withTextContent = (
    config: HofConfig,
    dependencies?: {
        getPageTextContentService?: GetPageTextContentService
    }
): GetServerSideProps => {
    const getPageTextContentService =
        dependencies?.getPageTextContentService ?? getPageTextContent

    const { props, routeId, getServerSideProps } = config

    return async (context: GetServerSidePropsContext): Promise<any> => {
        const locale: string = selectLocale(context)

        /**
         * Get data from services
         */
        try {
            const [pageTextContentData] = await Promise.all([
                getPageTextContentService({ id: routeId, locale }),
            ])

            props.contentText = pageTextContentData.data
        } catch (error) {
            console.log(error)
        }

        return await getServerSideProps(context)
    }
}
