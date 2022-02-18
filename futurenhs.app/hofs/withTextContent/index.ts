import { GetServerSideProps } from 'next';

import { selectLocale, selectProps } from '@selectors/context';
import { getPageTextContent } from '@services/getPageTextContent';
import { GetPageTextContentService } from '@services/getPageTextContent';
import { GetServerSidePropsContext, HofConfig } from '@appTypes/next';

export const withTextContent = (config: HofConfig, dependencies?: {
    getPageTextContentService?: GetPageTextContentService,
}): GetServerSideProps => {

    const getPageTextContentService = dependencies?.getPageTextContentService ?? getPageTextContent;

    const { routeId, getServerSideProps } = config;

    return async (context: GetServerSidePropsContext): Promise<any> => {

        const locale: string = selectLocale(context);
        const props: any = selectProps(context)

        /**
         * Get data from services
         */
        try {

            const [
                pageTextContentData
            ] = await Promise.all([
                getPageTextContentService({
                    id: routeId,
                    locale: locale
                })
            ]);

            props.contentText = pageTextContentData.data;

        } catch (error) {

            console.log(error);

        }

        context.props = props;

        return await getServerSideProps(context);

    }

}
