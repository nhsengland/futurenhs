import { GetServerSideProps } from 'next';

import { withAuth } from '@hofs/withAuth';
import { getGroups } from '@services/getGroups';
import { getPageTextContent } from '@services/getPageTextContent';
import { selectLocale, selectProps } from '@selectors/context';
import { selectUser, selectPagination } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupListingTemplate } from '@components/_pageTemplates/GroupListingTemplate';

const routeId: string = '8190d347-e29a-4577-baa8-446bcae428d9';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth({
    getServerSideProps: async (context: GetServerSidePropsContext) => {

        /**
         * Get data from request context
         */
        const user: User = selectUser(context);
        const locale: string = selectLocale(context);
        const initialPageNumber: number = selectPagination(context).pageNumber ?? 1;
        const initialPageSize: number = selectPagination(context).pageSize ?? 10;

        let props: any = selectProps(context);

        /**
         * Get data from services
         */
        try {

            const [
                pageTextContent,
                groupsList,
            ] = await Promise.all([
                getPageTextContent({
                    id: routeId,
                    locale: locale
                }),
                getGroups({
                    user: user,
                    isMember: false,
                    pagination: {
                        pageNumber: initialPageNumber,
                        pageSize: initialPageSize
                    }
                })
            ]);

            props.text = pageTextContent.data ?? null;
            props.groupsList = groupsList.data ?? [];
            props.pagination = groupsList.pagination ?? null;
        
        } catch (error) {

            console.log(error);
            
            props.errors = error;

        }

        /**
         * Return data to page template
         */
        return {
            props: props
        }

    }
});

/**
 * Export page template
 */
export default GroupListingTemplate;
