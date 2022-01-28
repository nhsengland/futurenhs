import { GetServerSideProps } from 'next';

import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { withAuth } from '@hofs/withAuth';
import { withTextContent } from '@hofs/withTextContent';
import { selectLocale, selectProps } from '@selectors/context';
import { getPageTextContent } from '@services/getPageTextContent';
import { GetServerSidePropsContext } from '@appTypes/next';

import { HomeTemplate } from '@components/_pageTemplates/HomeTemplate';
import { Props } from '@components/_pageTemplates/HomeTemplate/interfaces';

const routeId: string = '749bd865-27b8-4af6-960b-3f0458f8e92f';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth({
    getServerSideProps: withTextContent({
        routeId: routeId,
        getServerSideProps: async (context: GetServerSidePropsContext) => {

            let props: Props = selectProps(context);

            /**
             * Return data to page template
             */
            return {
                props: getJsonSafeObject({
                    object: props
                })
            }

        }
    })
});

/**
 * Export page template
 */
export default HomeTemplate;