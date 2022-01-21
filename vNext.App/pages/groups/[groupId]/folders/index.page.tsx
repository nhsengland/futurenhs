import { GetServerSideProps } from 'next';

import { routeParams } from '@constants/routes';
import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { getGroupFolder } from '@services/getGroupFolder';
import { getGroupFolders } from '@services/getGroupFolders';
import { selectUser, selectPagination, selectParam } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupFoldersTemplate } from '@components/_pageTemplates/GroupFoldersTemplate';

const routeId: string = '8b74608e-e22d-4dd9-9501-1946ac27e133';

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

            let { props } = context;

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
                    getGroupFolders({
                        user: user,
                        groupId: groupId,
                        folderId: folderId,
                        pagination: {
                            pageNumber: initialPageNumber,
                            pageSize: initialPageSize
                        }
                    })
                ]);

                props.files = groupFolderContents.data ?? [];
                props.pagination = groupFolderContents.pagination ?? null;
            
            } catch (error) {
                
                props.errors = error?.message ?? 'Error';

            }

            /**
             * Return data to page template
             */
            return {
                props: props
            }

        }
    })
});

/**
 * Export page template
 */
export default GroupFoldersTemplate;