import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps';
import { routeParams } from '@constants/routes';
import { actions as actionConstants } from '@constants/actions';
import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { withForms } from '@hofs/withForms';
import { selectCsrfToken, selectBody, selectProps, selectParam, selectUser } from '@selectors/context';
import { postGroupDiscussion } from '@services/postGroupDiscussion';
import { validate } from '@helpers/validators';
import { selectFormDefaultFields } from '@selectors/forms';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { createDiscussionForm } from '@formConfigs/create-discussion';
import { GroupCreateDiscussionTemplate } from '@components/_pageTemplates/GroupCreateDiscussionTemplate';
import { Props } from '@components/_pageTemplates/GroupCreateDiscussionTemplate/interfaces';

const routeId: string = 'fcf3d540-9a55-418c-b317-a14146ae075f';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth({
    getServerSideProps: withGroup({
        routeId: routeId,
        getServerSideProps: withForms({
            routeId: routeId,
            getServerSideProps: async (context: GetServerSidePropsContext) => {

                const user: User = selectUser(context);
                const groupId: string = selectParam(context, routeParams.GROUPID);
                const csrfToken: string = selectCsrfToken(context);
                const body: any = selectBody(context);
                const props: Props = selectProps(context);

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

                            const submission = await postGroupDiscussion({ groupId, user, csrfToken, body });

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
                                props.forms[createDiscussionForm.id].initialValues = body;

                            } else {

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
});

/**
 * Export page template
 */
export default GroupCreateDiscussionTemplate;