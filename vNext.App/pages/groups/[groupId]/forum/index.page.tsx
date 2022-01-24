import { GetServerSideProps } from 'next';

import { routeParams } from '@constants/routes';
import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { getGroupDiscussions } from '@services/getGroupDiscussions';
import { selectProps } from '@selectors/context';
import { selectUser, selectPagination, selectParam } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupForumTemplate } from '@components/_pageTemplates/GroupForumTemplate';

const routeId: string = 'd7752e9e-4f47-41ec-bc07-70508d8dcd9b';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth({
    routeId: routeId,
    getServerSideProps: withGroup({
        routeId: routeId,
        getServerSideProps: async (context: GetServerSidePropsContext) => {

            let props: any = selectProps(context);

            const user: User = selectUser(context);
            const groupId: string = selectParam(context, routeParams.GROUPID);
            const initialPageNumber: number = selectPagination(context).pageNumber ?? 1;
            const initialPageSize: number = selectPagination(context).pageSize ?? 5;

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

            } catch (error) {

                props.errors = error?.message ?? 'Error';

            }

            /**
             * Return data to page template
             */
            return {
                props: props
            }

        }
    })
});

/**
 * Export page template
 */
export default GroupForumTemplate;
