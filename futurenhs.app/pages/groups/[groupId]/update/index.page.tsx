import { GetServerSideProps } from 'next';

import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps';
import { getStandardServiceHeaders } from '@helpers/fetch';
import { routeParams } from '@constants/routes';
import { requestMethods } from '@constants/fetch';
import { formTypes } from '@constants/forms';
import { actions } from '@constants/actions';
import { layoutIds, groupTabIds } from '@constants/routes';
import { themes, defaultThemeId } from '@constants/themes';
import { withUser } from '@hofs/withUser';
import { withRoutes } from '@hofs/withRoutes';
import { withTextContent } from '@hofs/withTextContent';
import { withGroup } from '@hofs/withGroup';
import { withForms } from '@hofs/withForms';
import { selectMultiPartFormData, selectCsrfToken, selectParam, selectUser, selectRequestMethod } from '@selectors/context';
import { getGroup } from '@services/getGroup';
import { putGroup } from '@services/putGroup';
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
    getServerSideProps: withRoutes({
        props,
        getServerSideProps: withGroup({
            props,
            getServerSideProps: withTextContent({
                props,
                routeId,
                getServerSideProps: withForms({
                    props,
                    routeId,
                    getServerSideProps: async (context: GetServerSidePropsContext) => {

                        const csrfToken: string = selectCsrfToken(context);
                        const formData: any = selectMultiPartFormData(context);
                        const groupId: string = selectParam(context, routeParams.GROUPID);
                        const user: User = selectUser(context);
                        const requestMethod: requestMethods = selectRequestMethod(context);

                        props.layoutId = layoutIds.GROUP;
                        props.tabId = groupTabIds.INDEX;

                        /**
                         * Get data from services
                         */
                        try {

                            const [group] = await Promise.all([getGroup({ user, groupId, isForEdit: true })]);
                            const etag = group.headers.get('etag');

                            props.etag = etag;
                            props.forms[formTypes.UPDATE_GROUP].initialValues = {
                                'Name': props.entityText.title,
                                'Strapline': props.entityText.strapLine,
                                'ImageId': '',
                                'ThemeId': props.themeId && themes[props.themeId] ? [props.themeId] : [defaultThemeId]
                            };

                            /**
                             * Handle server-side form post
                             */
                            if (formData && requestMethod === requestMethods.POST) {

                                const headers = { 
                                    ...getStandardServiceHeaders({ csrfToken, etag }),
                                    ...formData.getHeaders()
                                };

                                await putGroup({ groupId, user, headers, body: formData });

                                return {
                                    redirect: {
                                        permanent: false,
                                        destination: props.routes.groupRoot
                                    }
                                }

                            }

                        } catch (error) {

                            if (error.data?.status) {

                                props.forms[updateGroupForm.id].errors = error.data.body || {
                                    _error: error.data.statusText
                                };

                            } else {

                                return handleSSRErrorProps({ props, error });

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
        })
    })
});

/**
 * Export page template
 */
export default GroupUpdateTemplate;