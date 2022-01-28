import { GetServerSideProps } from 'next';

import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { withTextContent } from '@hofs/withTextContent'
import { selectProps } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';

import { GenericContentTemplate } from '@components/_pageTemplates/GenericContentTemplate';
import { Props } from '@components/_pageTemplates/GenericContentTemplate/interfaces';

const routeId: string = 'c0e49c49-f9cb-40c0-bafe-4f603b495b1f';

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