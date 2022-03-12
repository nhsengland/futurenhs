import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { withUser } from '@hofs/withUser';
import { withTextContent } from '@hofs/withTextContent';
import { withRoutes } from '@hofs/withRoutes';
import { GetServerSidePropsContext } from '@appTypes/next';

import { GenericContentTemplate } from '@components/_pageTemplates/GenericContentTemplate';
import { Props } from '@components/_pageTemplates/GenericContentTemplate/interfaces';

const routeId: string = 'fec95cc7-3450-4266-a20a-91e303e58944';
const props: Partial<Props> = {};

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withUser({
    props,
    isRequired: false,
    getServerSideProps: withRoutes({
        props,
        getServerSideProps: withTextContent({
            props,
            routeId,
            getServerSideProps: async (context: GetServerSidePropsContext) => {
    
                /**
                 * Return data to page template
                 */
                return handleSSRSuccessProps({ props });
    
            }
        })
    })
});

/**
 * Export page template
 */
export default GenericContentTemplate;
