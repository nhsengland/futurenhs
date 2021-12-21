import { GetServerSideProps } from 'next';

import { getPageContent } from '@services/getPageContent';
import { selectUser, selectLocale } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GenericContentTemplate } from '@components/_pageTemplates/GenericContentTemplate';
import { Props } from '@components/_pageTemplates/GenericContentTemplate/interfaces';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (context: GetServerSidePropsContext) => {

    const id: string = 'c0e49c49-f9cb-40c0-bafe-4f603b495b1f';

    /**
     * Get data from request context
     */
    const user: User = selectUser(context);
    const locale: string = selectLocale(context);

    /**
     * Create page data
     */
    const props: Props = {
        id: id,
        user: user,
        content: null
    };

    /**
     * Get data from services
     */
    try {

        const [
            pageContent
        ] = await Promise.all([
            getPageContent({
                id: id,
                locale: locale
            })
        ]);

        props.content = pageContent.data;
    
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