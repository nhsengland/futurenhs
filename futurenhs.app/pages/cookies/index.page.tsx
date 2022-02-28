import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { withUser } from '@hofs/withUser';
import { withTextContent } from '@hofs/withTextContent'
import { GetServerSidePropsContext } from '@appTypes/next';

import { GenericContentTemplate } from '@components/_pageTemplates/GenericContentTemplate';
import { Props } from '@components/_pageTemplates/GenericContentTemplate/interfaces';

const routeId: string = 'c1ffa11a-c06a-4210-96f5-59e9f7f8fff5';
const props: Partial<Props> = {};

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withUser({
    props,
    isRequired: false,
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
});

/**
 * Export page template
 */
export default GenericContentTemplate;
