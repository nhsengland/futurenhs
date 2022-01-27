import { GetServerSideProps } from 'next';

import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { getPageTextContent } from '@services/getPageTextContent';
import { selectUser, selectLocale } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GenericContentTemplate } from '@components/_pageTemplates/GenericContentTemplate';
import { Props } from '@components/_pageTemplates/GenericContentTemplate/interfaces';

const routeId: string = '75a8c71a-f29b-4893-9939-fb4ee595c9a5';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (context: GetServerSidePropsContext) => {

    /**
     * Get data from request context
     */
    const user: User = selectUser(context);
    const locale: string = selectLocale(context);

    /**
     * Create page data
     */
    const props: Props = {
        id: routeId,
        user: user,
        text: null
    };

    /**
     * Get data from services
     */
    try {

        const [
            pageTextContent
        ] = await Promise.all([
            getPageTextContent({
                id: routeId,
                locale: locale
            })
        ]);

        props.text = pageTextContent.data;
        props.errors = [...props.errors, ...pageTextContent.errors];
    
    } catch (error) {
        
            props.errors = [{
                error: error.message
            }];

    }

    /**
     * Return data to page template
     */
    return {
        props: getJsonSafeObject({
            object: props
        })
    }

}

/**
 * Export page template
 */
export default GenericContentTemplate;
