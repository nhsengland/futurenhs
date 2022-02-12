import { GetServerSideProps } from 'next';

import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { withTextContent } from '@hofs/withTextContent'
import { selectProps } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';

import { GenericContentTemplate } from '@components/_pageTemplates/GenericContentTemplate';
import { Props } from '@components/_pageTemplates/GenericContentTemplate/interfaces';

const routeId: string = 'cc4d07b6-9c05-4bd6-9c1b-ad32a947e7be';

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
