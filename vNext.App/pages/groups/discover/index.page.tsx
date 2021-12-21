import { GetServerSideProps } from 'next';

import { withAuth } from '@hofs/withAuth';
import { getPageContent } from '@services/getPageContent';
import { getGroups } from '@services/getGroups';
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

    const id: string = '8190d347-e29a-4577-baa8-446bcae428d9';

    /**
     * Get data from request context
     */
    const user: User = selectUser(context);
    const locale: string = selectLocale(context);
    const initialPageNumber: number = selectPagination(context).pageNumber ?? 1;
    const initialPageSize: number = selectPagination(context).pageSize ?? 10;

    /**
     * Get data from services
     */
    const { data: content, errors } = await getPageContent({
        id: id,
        locale: locale
    });

    const { data: groupsList, pagination } = await getGroups({
        user: user,
        filters: {
            isMember: false
        },
        pagination: {
            pageNumber: initialPageNumber,
            pageSize: initialPageSize
        }
    });

    /**
     * Return data to page template
     */
    return {
        props: {
            id: id,
            user: user,
            content: content,
            groupsList: groupsList,
            pagination: pagination
        } as Props
    }

});

/**
 * Export page template
 */
export default GroupListingTemplate;