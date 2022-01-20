import { GetServerSideProps } from 'next';

import { getPageTextContent } from '@services/getPageTextContent';
import { selectUser, selectLocale } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GenericContentTemplate } from '@components/_pageTemplates/GenericContentTemplate';
import { Props } from '@components/_pageTemplates/GenericContentTemplate/interfaces';

const routeId: string = 'd9a68fe8-e2bf-4ce0-bc1e-9166b9b54a30';

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
    
    } catch (error) {
        
        props.errors = error;

    }

    /**
     * Return data to page template
     */
    return {
        props: props
    }

}

/**
 * Export page template
 */
export default GenericContentTemplate;
