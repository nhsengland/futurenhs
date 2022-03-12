import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps';
import { layoutIds, groupTabIds } from '@constants/routes';
import { routeParams } from '@constants/routes';
import { actions as actionConstants } from '@constants/actions';
import { withUser } from '@hofs/withUser';
import { withRoutes } from '@hofs/withRoutes';
import { withGroup } from '@hofs/withGroup';
import { withForms } from '@hofs/withForms';
import { selectCsrfToken, selectFormData, selectParam, selectUser, selectQuery } from '@selectors/context';
import { postGroupFolder } from '@services/postGroupFolder';
import { getGroupFolder } from '@services/getGroupFolder';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { createFolderForm } from '@formConfigs/create-folder';
import { GroupCreateFolderTemplate } from '@components/_pageTemplates/GroupCreateFolderTemplate';
import { Props } from '@components/_pageTemplates/GroupCreateFolderTemplate/interfaces';
import { withTextContent } from '@hofs/withTextContent';

const routeId: string = 'c1bc7b37-762f-4ed8-aed2-79fcd0e5d5d2';
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
                        const folderId: string = selectQuery(context, routeParams.FOLDERID);
                        const csrfToken: string = selectCsrfToken(context);
                        const formData: any = selectFormData(context);
    
                        props.layoutId = layoutIds.GROUP;
                        props.tabId = groupTabIds.FILES;
                        props.folderId = folderId;
                        props.folder = null;
    
                        if (!props.actions?.includes(actionConstants.GROUPS_FOLDERS_ADD)) {
    
                            return {
                                notFound: true
                            }
    
                        }
    
                        /**
                         * Get data from services
                         */
                        if (folderId) {
    
                            try {
    
                                const [groupFolder] = await Promise.all([getGroupFolder({ user, groupId, folderId })]);
    
                                props.folder = groupFolder.data;
    
                            } catch (error) {
    
                                return handleSSRErrorProps({ props, error });
    
                            }
    
                        }
    
                        /**
                         * handle server-side form POST
                         */
                        if (formData) {
    
                            props.forms[createFolderForm.id].initialValues = formData;
    
                            try {
    
                                await postGroupFolder({ groupId, folderId, user, csrfToken, body: formData });
    
                                return {
                                    props: {},
                                    redirect: {
                                        permanent: false,
                                        destination: props.routes.groupFoldersRoot
                                    }
                                }
    
                            } catch (error) {
    
                                if (error.data?.status) {
    
                                    props.forms[createFolderForm.id].errors = error.data.body || {
                                        _error: error.data.statusText
                                    };
    
                                } else {
    
                                    return handleSSRErrorProps({ props, error });
    
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
export default GroupCreateFolderTemplate;