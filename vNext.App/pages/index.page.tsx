import { GetServerSideProps } from 'next';

import { HomeTemplate } from '@components/_pageTemplates/HomeTemplate';
import { withAuth } from '@hofs/withAuth';
import { selectLocale, selectProps } from '@selectors/context';
import { getPageContent } from '@services/getPageContent';
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
                pageContent
            ] = await Promise.all([
                getPageContent({
                    id: routeId,
                    locale: locale
                })
            ]);

            props.content = pageContent.data;
        
        } catch (error) {
            
            props.errors = error;

        }

        /**
         * Return data to page template
         */
        return {
            props: props
        }

    }
});

/**
 * Export page template
 */
export default HomeTemplate;