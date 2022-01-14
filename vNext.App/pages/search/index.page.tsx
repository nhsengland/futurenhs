import { GetServerSideProps } from 'next';

import { getPageContent } from '@services/getPageContent';
import { getSearchResults } from '@services/getSearchResults';
import { selectUser, selectLocale, selectSearchTerm } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { SearchListingTemplate } from '@components/_pageTemplates/SearchListingTemplate';
import { Props } from '@components/_pageTemplates/SearchListingTemplate/interfaces';

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
    const term: string | string [] = selectSearchTerm(context);

    /**
     * Create page data
     */
    const props: Props = {
        id: id,
        user: user,
        content: null,
        term: term,
        resultsList: []
    };

    /**
     * Get data from services
     */
    try {

        const [
            pageContent,
            searchResults
        ] = await Promise.all([
            getPageContent({
                id: id,
                locale: locale
            }),
            getSearchResults({
                user: user,
                term: term as string
            })
        ]);

        props.content = pageContent.data;
        props.resultsList = searchResults.data;
    
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
export default SearchListingTemplate;
