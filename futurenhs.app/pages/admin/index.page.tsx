import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { withTextContent } from '@hofs/withTextContent'
import { selectProps } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';

import { AdminDashboardTemplate } from '@components/_pageTemplates/AdminDashboardTemplate';
import { Props } from '@components/_pageTemplates/AdminDashboardTemplate/interfaces';

const routeId: string = '9e86c5cc-6836-4319-8d9d-b96249d4c909';

/**
 * Get props to inject into page on the initial server-side request
 */
 export const getServerSideProps: GetServerSideProps = withTextContent({
    routeId: routeId,
    getServerSideProps: async (context: GetServerSidePropsContext) => {

        const props: Props = selectProps(context);

        /**
         * Return data to page template
         */
        return handleSSRSuccessProps({ props });

    }
});

/**
 * Export page template
 */
export default AdminDashboardTemplate;
