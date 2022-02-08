import { GetServerSideProps } from 'next';

import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { routeParams } from '@constants/routes';
import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { withTextContent } from '@hofs/withTextContent';
import { getGroupDiscussions } from '@services/getGroupDiscussions';
import { selectUser, selectPagination, selectParam, selectProps } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupForumTemplate } from '@components/_pageTemplates/GroupForumTemplate';
import { Props } from '@components/_pageTemplates/GroupForumTemplate/interfaces';

const routeId: string = 'd7752e9e-4f47-41ec-bc07-70508d8dcd9b';

export const whatever = async (context: GetServerSidePropsContext) => {

    const user: User = selectUser(context);
    const groupId: string = selectParam(context, routeParams.GROUPID);
    const initialPageNumber: number = selectPagination(context).pageNumber ?? 1;
    const initialPageSize: number = selectPagination(context).pageSize ?? 5;

    let props: Props = selectProps(context);

    /**
     * Get data from services
     */
    try {

        const [
            groupDiscussions
        ] = await Promise.all([
            getGroupDiscussions({
                user: user,
                groupId: groupId,
                pagination: {
                    pageNumber: initialPageNumber,
                    pageSize: initialPageSize
                }
            }),

        ]);

        props.discussionsList = groupDiscussions.data;
        props.pagination = groupDiscussions.pagination;
        props.errors = [...groupDiscussions.errors];

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

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth({
    routeId: routeId,
    getServerSideProps: withGroup({
        routeId: routeId,
        getServerSideProps: withTextContent({
            routeId: routeId,
            getServerSideProps: whatever
        })
    })
});

/**
 * Export page template
 */
export default GroupForumTemplate;
