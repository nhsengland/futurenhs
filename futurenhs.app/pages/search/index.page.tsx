import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps';
import { withAuth } from '@hofs/withAuth';
import { withTextContent } from '@hofs/withTextContent';
import { getSearchResults } from '@services/getSearchResults';
import { selectQuery, selectPagination } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { Pagination } from '@appTypes/pagination';

import { SearchListingTemplate } from '@components/_pageTemplates/SearchListingTemplate';
import { Props } from '@components/_pageTemplates/SearchListingTemplate/interfaces';

const routeId: string = '246485b1-2a13-4844-95d0-1fb401c8fdea';
const props: Partial<Props> = {};

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth({
    props,
    getServerSideProps: withTextContent({
        props,
        routeId: routeId,
        getServerSideProps: async (context: GetServerSidePropsContext) => {

            /**
             * Get data from request context
             */
            const term: string = selectQuery(context, 'term');
            const pagination: Pagination = selectPagination(context);

            const minLength: number = 3;

            /**
             * Get data from services
             */
            try {

                const [searchResults] = await Promise.all([getSearchResults({ term, pagination, minLength })]);

                props.term = term;
                props.minLength = minLength;
                props.resultsList = searchResults.data ?? [];
                props.pagination = searchResults.pagination;

            } catch (error) {

                return handleSSRErrorProps({ props, error });

            }

            /**
             * Return data to page template
             */
            return handleSSRSuccessProps({ props });

        }

    })
});

/**
 * Export page template
 */
export default SearchListingTemplate;
