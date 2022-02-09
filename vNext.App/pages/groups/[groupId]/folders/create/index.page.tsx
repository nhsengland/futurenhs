import { GetServerSideProps } from 'next';

import { routeParams } from '@constants/routes';
import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { actions as actionConstants } from '@constants/actions';
import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { selectCsrfToken, selectBody, selectParam, selectUser, selectProps, selectQuery } from '@selectors/context';
import { postGroupFolder } from '@services/postGroupFolder';
import { getGroupFolder } from '@services/getGroupFolder';
import { getServiceResponseErrors } from '@helpers/services/getServiceResponseErrors';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { createFolderForm } from '@formConfigs/create-folder';
import { GroupCreateFolderTemplate } from '@components/_pageTemplates/GroupCreateFolderTemplate';
import { Props } from '@components/_pageTemplates/GroupCreateFolderTemplate/interfaces';

const routeId: string = 'c1bc7b37-762f-4ed8-aed2-79fcd0e5d5d2';

/**
 * Get props to inject into page on the initial server-side request
 */
 export const getServerSideProps: GetServerSideProps = withAuth({
    getServerSideProps: withGroup({
            routeId: routeId,
            getServerSideProps: async (context: GetServerSidePropsContext) => {

                const user: User = selectUser(context);
                const groupId: string = selectParam(context, routeParams.GROUPID);
                const folderId: string = selectQuery(context, routeParams.FOLDERID);
                const csrfToken: string = selectCsrfToken(context);
                const formPost: any = selectBody(context);

                let props: Props = selectProps(context);

                props.csrfToken = csrfToken;
                props.forms = {
                    [createFolderForm.id]: createFolderForm
                };

                if(!props.actions?.includes(actionConstants.GROUPS_FOLDERS_ADD)){

                    return {
                        notFound: true
                    }

                }

                /**
                 * Get data from services
                 */
                if(folderId){

                    const [
                        groupFolder
                    ] = await Promise.all([
                        getGroupFolder({
                            user: user,
                            groupId: groupId,
                            folderId: folderId
                        })
                    ]);
    
                    if (getServiceResponseErrors({
                        serviceResponseList: [groupFolder],
                        matcher: (code) => Number(code) === 404
                    }).length > 0) {
    
                        return {
                            notFound: true
                        }
    
                    }

                    props.folder = groupFolder.data;

                }

                if(formPost){

                    try {

                        const submission = await postGroupFolder({
                            groupId: groupId,
                            user: user,
                            csrfToken: csrfToken,
                            body: {
                                formId: createFolderForm.id,
                                ...formPost
                            }
                        });

                    } catch(error){

                        if(error.data?.status === 400){

                            props.forms[createFolderForm.id].errors = error.data.body;
                            props.forms[createFolderForm.id].initialValues = formPost;

                        } else {

                            props.forms[createFolderForm.id].errors = {
                                [error.data.status]: error.data.statusText
                            };

                        }

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
});

/**
 * Export page template
 */
export default GroupCreateFolderTemplate;