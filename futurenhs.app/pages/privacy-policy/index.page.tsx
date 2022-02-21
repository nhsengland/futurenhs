import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { withTextContent } from '@hofs/withTextContent'
import { GetServerSidePropsContext } from '@appTypes/next';

import { GenericContentTemplate } from '@components/_pageTemplates/GenericContentTemplate';
import { Props } from '@components/_pageTemplates/GenericContentTemplate/interfaces';

const routeId: string = 'cc4d07b6-9c05-4bd6-9c1b-ad32a947e7be';
const props: Partial<Props> = {};

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withTextContent({
    props,
    routeId,
    getServerSideProps: async (context: GetServerSidePropsContext) => {

        /**
         * Return data to page template
         */
        return handleSSRSuccessProps({ props });

    }
});

/**
 * Export page template
 */
export default GenericContentTemplate;
