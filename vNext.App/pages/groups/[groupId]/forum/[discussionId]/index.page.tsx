import { GetServerSideProps } from 'next';

import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { routeParams } from '@constants/routes';
import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { withTextContent } from '@hofs/withTextContent';
import { getGroupDiscussion } from '@services/getGroupDiscussion';
import { postGroupDiscussionComment } from '@services/postGroupDiscussionComment';
import { getGroupDiscussionCommentsWithReplies } from '@services/getGroupDiscussionCommentsWithReplies';
import { selectUser, selectParam, selectProps, selectPagination, selectCsrfToken, selectBody } from '@selectors/context';
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
                const formPost: any = selectBody(context);

                let props: Props = selectProps(context);

                props.csrfToken = csrfToken;
                props.discussionId = discussionId;
                props.forms = {
                    [createDiscussionCommentForm.id]: createDiscussionCommentForm
                };

                if(formPost){

                    try {

                        const submission = await postGroupDiscussionComment({
                            groupId: groupId,
                            discussionId: discussionId,
                            user: user,
                            csrfToken: csrfToken,
                            body: {
                                formId: createDiscussionCommentForm.id,
                                ...formPost
                            }
                        });

                    } catch(error){

                        if (error.name === 'ServiceError') {

                            if(error.data?.status === 400){

                                props.forms[createDiscussionCommentForm.id].errors = error.data.body;
                                props.forms[createDiscussionCommentForm.id].initialValues = formPost;
    
                            } else {
    
                                props.forms[createDiscussionCommentForm.id].errors = {
                                    [error.data.status]: error.data.statusText
                                };
    
                            }
        
                        }

                    }

                }

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
                        getGroupDiscussionCommentsWithReplies({
                            user: user,
                            groupId: groupId,
                            discussionId: discussionId,
                            pagination: pagination
                        })
                    ]);

                    props.discussion = groupDiscussion.data;
                    props.discussionCommentsList = groupDiscussionComments.data;
                    props.pagination = groupDiscussionComments.pagination;

                } catch (error) {

                    if (error.name === 'ServiceError') {

                        if(error.data.status === 404){

                            return {
                                notFound: true
                            }

                        }

                        props.errors = [{
                            [error.data.status]: error.data.statusText
                        }];

                    }

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
