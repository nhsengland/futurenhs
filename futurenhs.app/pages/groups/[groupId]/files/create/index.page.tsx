import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps';
import { layoutIds, groupTabIds } from '@constants/routes';
import { requestMethods } from '@constants/fetch';
import { routeParams } from '@constants/routes';
import { actions as actionConstants } from '@constants/actions';
import { withUser } from '@hofs/withUser';
import { withRoutes } from '@hofs/withRoutes';
import { withGroup } from '@hofs/withGroup';
import { withForms } from '@hofs/withForms';
import { selectCsrfToken, selectFormData, selectParam, selectUser, selectQuery, selectRequestMethod } from '@selectors/context';
import { postGroupFile } from '@services/postGroupFile';
import { getGroupFolder } from '@services/getGroupFolder';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { createFileForm } from '@formConfigs/create-file';
import { GroupCreateFileTemplate } from '@components/_pageTemplates/GroupCreateFileTemplate';
import { Props } from '@components/_pageTemplates/GroupCreateFolderTemplate/interfaces';
import { withTextContent } from '@hofs/withTextContent';

const routeId: string = '2ff0717e-494f-4400-8c33-600c080e27b7';
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
            routeId,
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
                        const requestMethod: requestMethods = selectRequestMethod(context);
    
                        props.layoutId = layoutIds.GROUP;
                        props.tabId = groupTabIds.FILES;
    
                        if (!props.actions?.includes(actionConstants.GROUPS_FILES_ADD)) {
    
                            return {
                                notFound: true
                            }
    
                        }
    
                        /**
                         * Get data from services
                         */
                        try {
    
                            const [groupFolder] = await Promise.all([
                                getGroupFolder({ user, groupId, folderId })
                            ]);
    
                            props.folderId = folderId;
                            props.folder = groupFolder.data;
    
                        } catch (error) {
    
                            if(error.data?.status === 404){
    
                                return {
                                    notFound: true
                                }
    
                            } else {
    
                                return handleSSRErrorProps({ props, error });
    
                            }
    
                        }
    
                        /**
                         * handle server-side form POST
                         */
                        if (formData && requestMethod === requestMethods.POST) {
    
                            // TODO
    
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
export default GroupCreateFileTemplate;