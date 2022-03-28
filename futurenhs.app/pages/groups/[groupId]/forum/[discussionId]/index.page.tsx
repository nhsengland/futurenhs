import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps';
import { getServiceErrorDataValidationErrors } from '@services/index';
import { getStandardServiceHeaders } from '@helpers/fetch';
import { routeParams } from '@constants/routes';
import { layoutIds, groupTabIds } from '@constants/routes';
import { requestMethods } from '@constants/fetch';
import { withUser } from '@hofs/withUser';
import { withRoutes } from '@hofs/withRoutes';
import { withForms } from '@hofs/withForms';
import { withGroup } from '@hofs/withGroup';
import { withTextContent } from '@hofs/withTextContent';
import { getGroupDiscussion } from '@services/getGroupDiscussion';
import { postGroupDiscussionComment } from '@services/postGroupDiscussionComment';
import { postGroupDiscussionCommentReply } from '@services/postGroupDiscussionCommentReply';
import { getGroupDiscussionCommentsWithReplies } from '@services/getGroupDiscussionCommentsWithReplies';
import { selectUser, selectParam, selectPagination, selectFormData, selectRequestMethod, selectCsrfToken } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';
import { Pagination } from '@appTypes/pagination';

import { createDiscussionCommentForm } from '@formConfigs/create-discussion-comment';
import { createDiscussionCommentReplyForm } from '@formConfigs/create-discussion-comment-reply';
import { GroupDiscussionTemplate } from '@components/_pageTemplates/GroupDiscussionTemplate';
import { Props } from '@components/_pageTemplates/GroupDiscussionTemplate/interfaces';
import { formTypes } from '@constants/forms';
import { ServerSideFormData } from '@helpers/util/form';
import { FormErrors } from '@appTypes/form';

const routeId: string = 'f9658510-6950-43c4-beea-4ddeca277a5f';
const props: Partial<Props> = {};

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withUser({
    props,
    getServerSideProps: withRoutes({
        props,
        getServerSideProps: withGroup({
            props,
            getServerSideProps: withForms({
                props,
                routeId,
                getServerSideProps: withTextContent({
                    props,
                    routeId,
                    getServerSideProps: async (context: GetServerSidePropsContext) => {

                        const user: User = selectUser(context);
                        const groupId: string = selectParam(context, routeParams.GROUPID);
                        const discussionId: string = selectParam(context, routeParams.DISCUSSIONID);
                        const pagination: Pagination = selectPagination(context);
                        const formData: ServerSideFormData = selectFormData(context);
                        const requestMethod: requestMethods = selectRequestMethod(context);
                        const csrfToken: string = selectCsrfToken(context);

                        const commentForm: any = props.forms[createDiscussionCommentForm.id];
                        const replyForm: any = props.forms[createDiscussionCommentReplyForm.id];

                        props.discussionId = discussionId;
                        props.layoutId = layoutIds.GROUP;
                        props.tabId = groupTabIds.FORUM;

                        if (formData && requestMethod === requestMethods.POST) {

                            const formId = formData.get('_form-id');
                            const commentId: string = formData.get('_instance-id');
                            const headers = getStandardServiceHeaders({ csrfToken });

                            try {

                                if (formId === formTypes.CREATE_DISCUSSION_COMMENT) {

                                    commentForm.initialValues = formData.getAll();

                                    await postGroupDiscussionComment({ groupId, discussionId, user, headers, body: formData });

                                } else if (formId === formTypes.CREATE_DISCUSSION_COMMENT_REPLY) {

                                    replyForm.initialValues = formData.getAll();

                                    await postGroupDiscussionCommentReply({ groupId, discussionId, commentId, user, headers, body: formData });

                                }

                            } catch (error) {

                                const validationErrors: FormErrors = getServiceErrorDataValidationErrors(error);

                                if (validationErrors) {

                                    if(formId === formTypes.CREATE_DISCUSSION_COMMENT) {

                                        commentForm.errors = validationErrors;

                                    } else if(formId === formTypes.CREATE_DISCUSSION_COMMENT_REPLY) {

                                        replyForm.errors = validationErrors;

                                    }

                                } else {

                                    return handleSSRErrorProps({ props, error });

                                }

                            }

                        }

                        /**
                         * Get data from services
                         */
                        try {

                            const [groupDiscussion, groupDiscussionComments] = await Promise.all([
                                getGroupDiscussion({ user, groupId, discussionId }),
                                getGroupDiscussionCommentsWithReplies({ user, groupId, discussionId, pagination })
                            ]);

                            props.discussion = groupDiscussion.data;
                            props.discussionCommentsList = groupDiscussionComments.data;
                            props.pagination = groupDiscussionComments.pagination;
                            props.pageTitle = `${props.entityText.title} - ${props.discussion.text.title}`;


                        } catch (error) {

                            return handleSSRErrorProps({ props, error });

                        }

                        /**
                         * Return data to page template
                         */
                        return handleSSRSuccessProps({ props });

                    }
                })
            })
        })
    })
});

/**
 * Export page template
 */
export default GroupDiscussionTemplate;
