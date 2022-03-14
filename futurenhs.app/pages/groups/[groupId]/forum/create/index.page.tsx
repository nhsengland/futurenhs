import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps';
import { getServerSideMultiPartFormData } from '@helpers/util/form';
import { routeParams } from '@constants/routes';
import { requestMethods } from '@constants/fetch';
import { layoutIds, groupTabIds } from '@constants/routes';
import { actions as actionConstants } from '@constants/actions';
import { withUser } from '@hofs/withUser';
import { withRoutes } from '@hofs/withRoutes';
import { withGroup } from '@hofs/withGroup';
import { withForms } from '@hofs/withForms';
import { selectFormData, selectCsrfToken, selectParam, selectUser, selectRequestMethod } from '@selectors/context';
import { postGroupDiscussion } from '@services/postGroupDiscussion';
import { validate } from '@helpers/validators';
import { selectFormDefaultFields } from '@selectors/forms';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { createDiscussionForm } from '@formConfigs/create-discussion';
import { GroupCreateDiscussionTemplate } from '@components/_pageTemplates/GroupCreateDiscussionTemplate';
import { Props } from '@components/_pageTemplates/GroupCreateDiscussionTemplate/interfaces';
import { withTextContent } from '@hofs/withTextContent';

const routeId: string = 'fcf3d540-9a55-418c-b317-a14146ae075f';
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
                        const csrfToken: string = selectCsrfToken(context);
                        const formData: any = selectFormData(context);
                        const requestMethod: requestMethods = selectRequestMethod(context);
    
                        props.layoutId = layoutIds.GROUP;
                        props.tabId = groupTabIds.FORUM;
    
                        /**
                         * Return page not found if user doesn't have permissions to create a discussion
                         */
                        if (!props.actions?.includes(actionConstants.GROUPS_DISCUSSIONS_ADD)) {
    
                            return {
                                notFound: true
                            }
    
                        }
    
                        /**
                         * Handle server-side form post
                         */
                        if (formData && requestMethod === requestMethods.POST) {
    
                            const validationErrors: Record<string, string> = validate(formData, selectFormDefaultFields(props.forms, createDiscussionForm.id));
    
                            props.forms[createDiscussionForm.id].errors = validationErrors;
                            props.forms[createDiscussionForm.id].initialValues = formData;
    
                            if (Object.keys(validationErrors).length === 0) {
    
                                try {
    
                                    await postGroupDiscussion({ groupId, user, body: getServerSideMultiPartFormData(formData) as any });
    
                                    return {
                                        redirect: {
                                            permanent: false,
                                            destination: props.routes.groupForumRoot
                                        }
                                    }
    
                                } catch (error) {
    
                                    if (error.data?.status) {
    
                                        props.forms[createDiscussionForm.id].errors = error.data.body || {
                                            _error: error.data.statusText
                                        };
    
                                        return handleSSRErrorProps({ props, error });
    
                                    }
    
                                }
    
                            }
    
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
export default GroupCreateDiscussionTemplate;