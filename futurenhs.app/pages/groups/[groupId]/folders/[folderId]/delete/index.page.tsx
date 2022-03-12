import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps';
import { layoutIds, groupTabIds } from '@constants/routes';
import { routeParams } from '@constants/routes';
import { actions as actionConstants } from '@constants/actions';
import { withUser } from '@hofs/withUser';
import { withRoutes } from '@hofs/withRoutes';
import { withGroup } from '@hofs/withGroup';
import { deleteGroupFolder } from '@services/deleteGroupFolder';
import { selectParam, selectUser, selectQuery, selectCsrfToken } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupCreateFolderTemplate } from '@components/_pageTemplates/GroupCreateFolderTemplate';
import { Props } from '@components/_pageTemplates/GroupCreateFolderTemplate/interfaces';

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
            getServerSideProps: async (context: GetServerSidePropsContext) => {
    
                const user: User = selectUser(context);
                const groupId: string = selectParam(context, routeParams.GROUPID);
                const folderId: string = selectQuery(context, routeParams.FOLDERID);
                const csrfToken: string = selectCsrfToken(context);
    
                props.layoutId = layoutIds.GROUP;
                props.tabId = groupTabIds.FILES;
                props.folderId = folderId;
    
                /**
                 * Return not found if user does not have ability to delete folders in this group
                 */
                if (!props.actions?.includes(actionConstants.GROUPS_FOLDERS_DELETE)) {
    
                    return {
                        notFound: true
                    }
    
                }
    
                /**
                 * Attempt to delete group folder
                 */
                try { 
    
                    await deleteGroupFolder({ user, groupId, folderId, csrfToken });
                    
                    /**
                     * Redirect to home
                     */
                    return {
                        redirect: {
                            permanent: false,
                            destination: '/'
                        }
                    };
    
                } catch(error){
    
                    return handleSSRErrorProps({ error, props })
    
                }
    
            }
        })
    })
});

/**
 * Export page template
 */
export default GroupCreateFolderTemplate;