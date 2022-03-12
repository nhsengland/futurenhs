import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps';
import { routeParams } from '@constants/routes';
import { layoutIds, groupTabIds } from '@constants/routes';
import { validate } from '@helpers/validators';
import { selectFormDefaultFields } from '@selectors/forms';
import { getServerSideMultiPartFormData } from '@helpers/util/form';
import { withUser } from '@hofs/withUser';
import { withRoutes } from '@hofs/withRoutes';
import { withForms } from '@hofs/withForms';
import { withGroup } from '@hofs/withGroup';
import { withTextContent } from '@hofs/withTextContent';
import { getGroupDiscussion } from '@services/getGroupDiscussion';
import { postGroupDiscussionComment } from '@services/postGroupDiscussionComment';
import { getGroupDiscussionCommentsWithReplies } from '@services/getGroupDiscussionCommentsWithReplies';
import { selectUser, selectParam, selectPagination, selectFormData } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';
import { Pagination } from '@appTypes/pagination';

import { createDiscussionCommentForm } from '@formConfigs/create-discussion-comment';
import { GroupDiscussionTemplate } from '@components/_pageTemplates/GroupDiscussionTemplate';
import { Props } from '@components/_pageTemplates/GroupDiscussionTemplate/interfaces';

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
                        const formData: any = selectFormData(context);;
    
                        props.discussionId = discussionId;
                        props.layoutId = layoutIds.GROUP;
                        props.tabId = groupTabIds.FORUM;
    
                        if(formData){
            
                            const validationErrors: Record<string, string> = validate(formData, selectFormDefaultFields(props.forms, createDiscussionCommentForm.id));
    
                            props.forms[createDiscussionCommentForm.id].errors = validationErrors;
                            props.forms[createDiscussionCommentForm.id].initialValues = formData;
    
                            if(Object.keys(validationErrors).length === 0) {
    
                                try {
    
                                    await postGroupDiscussionComment({ groupId, discussionId, user, body: getServerSideMultiPartFormData(formData) as any });
    
                                } catch(error){
    
                                    if(error.data?.status){
    
                                        props.forms[createDiscussionCommentForm.id].errors = error.data.body || {
                                            _error: error.data.statusText
                                        };
                                        props.forms[createDiscussionCommentForm.id].initialValues = formData;
    
                                    } else {
    
                                        return handleSSRErrorProps({ props, error });
    
                                    }
    
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
