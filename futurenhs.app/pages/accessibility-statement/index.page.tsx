import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { withTextContent } from '@hofs/withTextContent'
import { GetServerSidePropsContext } from '@appTypes/next';

import { GenericContentTemplate } from '@components/_pageTemplates/GenericContentTemplate';
import { Props } from '@components/_pageTemplates/GenericContentTemplate/interfaces';

const routeId: string = '75a8c71a-f29b-4893-9939-fb4ee595c9a5';
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
