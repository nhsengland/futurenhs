import { GetServerSideProps } from 'next';

import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { HomeTemplate } from '@components/_pageTemplates/HomeTemplate';
import { withAuth } from '@hofs/withAuth';
import { selectLocale, selectProps } from '@selectors/context';
import { getPageTextContent } from '@services/getPageTextContent';
import { GetServerSidePropsContext } from '@appTypes/next';

const routeId: string = '749bd865-27b8-4af6-960b-3f0458f8e92f';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth({
    getServerSideProps: async (context: GetServerSidePropsContext) => {

        /**
         * Get data from request context
         */
        const locale: string = selectLocale(context);

        let props: any = selectProps(context);

        /**
         * Get data from services
         */
        try {

            const [
                pageTextContent
            ] = await Promise.all([
                getPageTextContent({
                    id: routeId,
                    locale: locale
                })
            ]);

            props.text = pageTextContent.data;
            props.errors = Object.assign(props.errors, pageTextContent.errors);
        
        } catch (error) {
            
            props.errors = {
                error: error.message
            };

        }

        /**
         * Return data to page template
         */
        return {
            props: getJsonSafeObject({
                object: props
            })
        }

    }
});

/**
 * Export page template
 */
export default HomeTemplate;