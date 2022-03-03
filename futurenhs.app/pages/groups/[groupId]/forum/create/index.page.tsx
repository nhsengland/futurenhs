import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps';
import { getServerSideMultiPartFormData } from '@helpers/util/form';
import { routeParams } from '@constants/routes';
import { layoutIds } from '@constants/routes';
import { actions as actionConstants } from '@constants/actions';
import { withUser } from '@hofs/withUser';
import { withGroup } from '@hofs/withGroup';
import { withForms } from '@hofs/withForms';
import { selectBody, selectCsrfToken, selectParam, selectUser } from '@selectors/context';
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
                    const body: any = selectBody(context);

                    props.layoutId = layoutIds.GROUP;
                    props.tabId = 'forum';

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
                    if (body) {

                        const validationErrors: Record<string, string> = validate(body, selectFormDefaultFields(props.forms, createDiscussionForm.id));

                        props.forms[createDiscussionForm.id].errors = validationErrors;
                        props.forms[createDiscussionForm.id].initialValues = body;

                        if (Object.keys(validationErrors).length === 0) {

                            try {

                            const formData: any = getServerSideMultiPartFormData(body);

                            await postGroupDiscussion({ groupId, user, body: formData });

                                return {
                                    props: {},
                                    redirect: {
                                        permanent: false,
                                        destination: `/groups/${context.params.groupId}/forum`
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
});

/**
 * Export page template
 */
export default GroupCreateDiscussionTemplate;