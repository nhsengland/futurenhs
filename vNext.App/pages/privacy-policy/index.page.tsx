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

    const id: string = 'cc4d07b6-9c05-4bd6-9c1b-ad32a947e7be';

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
