import { GetServerSideProps } from 'next';

import { withAuth } from '@hofs/withAuth';
import { getGroups } from '@services/getGroups';
import { getPageContent } from '@services/getPageContent';
import { selectLocale } from '@selectors/context';
import { selectUser, selectPagination } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupListingTemplate } from '@components/_pageTemplates/GroupListingTemplate';
import { Props } from '@components/_pageTemplates/GroupListingTemplate/interfaces';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth(async (context: GetServerSidePropsContext) => {

    const id: string = '3c745d1d-9742-459a-a2bb-7af14c2f291c';

    /**
     * Get data from request context
     */
    const user: User = selectUser(context);
    const locale: string = selectLocale(context);
    const initialPageNumber: number = selectPagination(context).pageNumber ?? 1;
    const initialPageSize: number = selectPagination(context).pageSize ?? 10;

    /**
     * Create page data
     */
    const props: Props = {
        id: id,
        user: user,
        content: null,
        groupsList: [],
        pagination: null,
        errors: null
    };

    /**
     * Get data from services
     */
    try {

        const [
            pageContent,
            groupsList,
        ] = await Promise.all([
            getPageContent({
                id: id,
                locale: locale
            }),
            getGroups({
                user: user,
                isMember: true,
                pagination: {
                    pageNumber: initialPageNumber,
                    pageSize: initialPageSize
                }
            })
        ]);

        props.content = pageContent.data;
        props.groupsList = groupsList.data;
        props.pagination = groupsList.pagination;
    
    } catch (error) {
        
        props.errors = error;

    }

    /**
     * Return data to page template
     */
    return {
        props: props
    }

});

/**
 * Export page template
 */
export default GroupListingTemplate;
