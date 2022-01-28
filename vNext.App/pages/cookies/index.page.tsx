import { GetServerSideProps } from 'next';

import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { withTextContent } from '@hofs/withTextContent'
import { selectProps } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';

import { GenericContentTemplate } from '@components/_pageTemplates/GenericContentTemplate';
import { Props } from '@components/_pageTemplates/GenericContentTemplate/interfaces';

const routeId: string = 'd9a68fe8-e2bf-4ce0-bc1e-9166b9b54a30';

/**
 * Get props to inject into page on the initial server-side request
 */
 export const getServerSideProps: GetServerSideProps = withTextContent({
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
});

/**
 * Export page template
 */
export default GenericContentTemplate;
