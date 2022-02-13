import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { withAuth } from '@hofs/withAuth';
import { withTextContent } from '@hofs/withTextContent';
import { selectProps } from '@selectors/context';
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

            const props: Props = selectProps(context);

            /**
             * Return data to page template
             */
            return handleSSRSuccessProps({ props });

        }
    })
});

/**
 * Export page template
 */
export default HomeTemplate;