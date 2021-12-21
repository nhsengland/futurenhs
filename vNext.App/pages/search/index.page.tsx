import { GetServerSideProps } from 'next';

import { getPageContent } from '@services/getPageContent';
import { selectUser, selectLocale } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { SearchTemplate } from '@components/_pageTemplates/SearchTemplate';
import { Props } from '@components/_pageTemplates/SearchTemplate/interfaces';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (context: GetServerSidePropsContext) => {

    const id: string = '246485b1-2a13-4844-95d0-1fb401c8fdea';

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
        content: null,
        term: context.query.term
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
export default SearchTemplate;
