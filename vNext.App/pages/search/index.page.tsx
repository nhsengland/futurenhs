import { GetServerSideProps } from 'next';

import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { withAuth } from '@hofs/withAuth';
import { getPageTextContent } from '@services/getPageTextContent';
import { getSearchResults } from '@services/getSearchResults';
import { selectLocale, selectQuery, selectProps, selectPagination } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { Pagination } from '@appTypes/pagination';

import { SearchListingTemplate } from '@components/_pageTemplates/SearchListingTemplate';
import { Props } from '@components/_pageTemplates/SearchListingTemplate/interfaces';

const routeId: string = '246485b1-2a13-4844-95d0-1fb401c8fdea';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth({
    getServerSideProps: async (context: GetServerSidePropsContext) => {

        /**
         * Get data from request context
         */
        const locale: string = selectLocale(context);
        const term: string = selectQuery(context, 'term');
        const pagination: Pagination = selectPagination(context);

        let props: Props = selectProps(context);

        /**
         * Get data from services
         */
        try {

            const [
                pageTextContent,
                searchResults
            ] = await Promise.all([
                getPageTextContent({
                    id: routeId,
                    locale: locale
                }),
                getSearchResults({
                    term: term,
                    pagination: pagination
                })
            ]);

            props.text = pageTextContent.data;
            props.term = term;
            props.resultsList = searchResults.data ?? [];
            props.pagination = searchResults.pagination;
            props.errors = [...props.errors, ...pageTextContent.errors, ...searchResults.errors];
        
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

});

/**
 * Export page template
 */
export default SearchListingTemplate;
