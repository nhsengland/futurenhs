import { GetServerSideProps } from 'next';

import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { routeParams } from '@constants/routes';
import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { withTextContent } from '@hofs/withTextContent';
import { getServiceResponseErrors } from '@helpers/services/getServiceResponseErrors';
import { getGroupDiscussion } from '@services/getGroupDiscussion';
import { getGroupDiscussionComments } from '@services/getGroupDiscussionComments';
import { selectUser, selectParam, selectProps, selectPagination, selectCsrfToken } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';
import { Pagination } from '@appTypes/pagination';

import { createDiscussionCommentForm } from '@formConfigs/create-discussion-comment';

import { GroupDiscussionTemplate } from '@components/_pageTemplates/GroupDiscussionTemplate';
import { Props } from '@components/_pageTemplates/GroupDiscussionTemplate/interfaces';

const routeId: string = 'f9658510-6950-43c4-beea-4ddeca277a5f';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth({
    getServerSideProps: withGroup({
        routeId: routeId,
        getServerSideProps: withTextContent({
            routeId: routeId,
            getServerSideProps: async (context: GetServerSidePropsContext) => {

                const user: User = selectUser(context);
                const groupId: string = selectParam(context, routeParams.GROUPID);
                const discussionId: string = selectParam(context, routeParams.DISCUSSIONID);
                const pagination: Pagination = selectPagination(context);
                const csrfToken: string = selectCsrfToken(context);

                let props: Props = selectProps(context);

                /**
                 * Get data from services
                 */
                try {

                    const [
                        groupDiscussion,
                        groupDiscussionComments
                    ] = await Promise.all([
                        getGroupDiscussion({
                            user: user,
                            groupId: groupId,
                            discussionId: discussionId
                        }),
                        getGroupDiscussionComments({
                            user: user,
                            groupId: groupId,
                            discussionId: discussionId,
                            pagination: pagination
                        })
                    ]);

                    if (getServiceResponseErrors({
                        serviceResponseList: [groupDiscussion],
                        matcher: (code) => Number(code) === 404
                    }).length > 0) {

                        return {
                            notFound: true
                        }

                    }

                    props.forms = {
                        [createDiscussionCommentForm.id]: createDiscussionCommentForm
                    };
                    props.csrfToken = csrfToken;
                    props.discussionId = discussionId;
                    props.discussion = groupDiscussion.data;
                    props.discussionComments = groupDiscussionComments.data;
                    props.pagination = groupDiscussionComments.pagination;
                    props.errors = [...props.errors, ...groupDiscussion.errors, ...groupDiscussionComments.errors];

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
        })
    })
});

/**
 * Export page template
 */
export default GroupDiscussionTemplate;
