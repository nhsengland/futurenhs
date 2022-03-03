import { GetServerSideProps } from 'next';

import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps';
import { getServerSideMultiPartFormData } from '@helpers/util/form';
import { routeParams } from '@constants/routes';
import { formTypes } from '@constants/forms';
import { actions } from '@constants/actions';
import { layoutIds } from '@constants/routes';
import { withUser } from '@hofs/withUser';
import { withGroup } from '@hofs/withGroup';
import { withForms } from '@hofs/withForms';
import { validate } from '@helpers/validators';
import { selectBody, selectCsrfToken, selectParam, selectUser } from '@selectors/context';
import { selectFormDefaultFields } from '@selectors/forms';
import { putGroupDetails } from '@services/putGroupDetails';
import { GetServerSidePropsContext } from '@appTypes/next';

import { updateGroupForm } from '@formConfigs/update-group';
import { GroupUpdateTemplate } from '@components/_pageTemplates/GroupUpdateTemplate';
import { Props } from '@components/_pageTemplates/GroupUpdateTemplate/interfaces';
import { User } from '@appTypes/user';

const routeId: string = '578dfcc6-857f-4eda-8779-1d9b110888c7';
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
            getServerSideProps: async (context: GetServerSidePropsContext) => {

                const csrfToken: string = selectCsrfToken(context);
                const body: any = selectBody(context);
                const groupId: string = selectParam(context, routeParams.GROUPID);
                const user: User = selectUser(context);

                props.forms[formTypes.UPDATE_GROUP].initialValues = {
                    'name': props.entityText.title,
                    'strapline': props.entityText.strapLine,
                    'themeId': [props.themeId]
                };
                props.layoutId = layoutIds.GROUP;
                props.tabId = 'index';

                /**
                 * Handle server-side form post
                 */
                if (body) {

                    const validationErrors: Record<string, string> = validate(body, selectFormDefaultFields(props.forms, updateGroupForm.id));

                    props.forms[updateGroupForm.id].errors = validationErrors;
                    props.forms[updateGroupForm.id].initialValues = body;

                    if (Object.keys(validationErrors).length === 0) {

                        try {

                            const formData: any = getServerSideMultiPartFormData(body);

                            await putGroupDetails({ groupId, user, csrfToken, body: formData });

                            return {
                                props: {},
                                redirect: {
                                    permanent: false,
                                    destination: `/groups/${context.params.groupId}`
                                }
                            }

                        } catch (error) {

                            if (error.data?.status) {

                                props.forms[updateGroupForm.id].errors = error.data.body || {
                                    _error: error.data.statusText
                                };
                                props.forms[updateGroupForm.id].initialValues = body;

                            } else {

                                return handleSSRErrorProps({ props, error });

                            }

                        }

                    }

                }

                /**
                 * Return data to page template
                 */
                return {
                    notFound: !props.actions.includes(actions.GROUPS_EDIT),
                    props: props
                }

            }
        })
    })
});

/**
 * Export page template
 */
export default GroupUpdateTemplate;