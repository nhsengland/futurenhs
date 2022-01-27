import { GetServerSideProps } from 'next';

import { getServiceResponseErrors } from '@helpers/services/getServiceResponseErrors';
import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { routeParams } from '@constants/routes';
import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { getGroupFolder } from '@services/getGroupFolder';
import { getGroupFolderContents } from '@services/getGroupFolderContents';
import { selectUser, selectPagination, selectParam, selectProps } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupFolderContentsTemplate } from '@components/_pageTemplates/GroupFolderContentsTemplate';
import { Props } from '@components/_pageTemplates/GroupFolderContentsTemplate/interfaces';

const routeId: string = '3ea9a707-4686-4129-a9fc-9041a6d5ae6e';

/**
 * Get props to inject into page on the initial server-side request
 */
 export const getServerSideProps: GetServerSideProps = withAuth({
    getServerSideProps: withGroup({
        routeId: routeId,
        getServerSideProps: async (context: GetServerSidePropsContext) => {

            const user: User = selectUser(context);
            const groupId: string = selectParam(context, routeParams.GROUPID);
            const folderId: string = selectParam(context, routeParams.FOLDERID);
            const initialPageNumber: number = selectPagination(context).pageNumber ?? 1;
            const initialPageSize: number = selectPagination(context).pageSize ?? 10;

            let props: Props = selectProps(context);

            /**
             * Get data from services
             */
            try {

                const [
                    groupFolder,
                    groupFolderContents
                ] = await Promise.all([
                    getGroupFolder({
                        user: user,
                        groupId: groupId,
                        folderId: folderId
                    }),
                    getGroupFolderContents({
                        user: user,
                        groupId: groupId,
                        folderId: folderId,
                        pagination: {
                            pageNumber: initialPageNumber,
                            pageSize: initialPageSize
                        }
                    })
                ]);

                if(getServiceResponseErrors({
                    serviceResponseList: [groupFolder],
                    matcher: (code) => code === 404
                }).length > 0){

                    return {
                        notFound: true
                    }

                }

                props.folderId = folderId;
                props.folder = groupFolder.data;
                props.folderContents = groupFolderContents.data ?? [];
                props.pagination = groupFolderContents.pagination;
                props.errors = Object.assign(props.errors, groupFolder.errors, groupFolderContents.errors);
            
            } catch (error) {
                
                props.errors = {
                    error: error.message
                };

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
export default GroupFolderContentsTemplate;
