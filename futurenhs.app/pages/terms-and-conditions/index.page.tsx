import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { withTextContent } from '@hofs/withTextContent'
import { GetServerSidePropsContext } from '@appTypes/next';

import { GenericContentTemplate } from '@components/_pageTemplates/GenericContentTemplate';
import { Props } from '@components/_pageTemplates/GenericContentTemplate/interfaces';

const routeId: string = 'c0e49c49-f9cb-40c0-bafe-4f603b495b1f';
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
