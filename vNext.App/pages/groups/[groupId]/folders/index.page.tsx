import { GetServerSideProps } from 'next';

import { routeParams } from '@constants/routes';
import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { getGroupFolders } from '@services/getGroupFolders';
import { selectUser, selectPagination, selectParam, selectProps } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupFoldersTemplate } from '@components/_pageTemplates/GroupFoldersTemplate';
import { Props } from '@components/_pageTemplates/GroupFoldersTemplate/interfaces';

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
            const initialPageNumber: number = selectPagination(context).pageNumber ?? 1;
            const initialPageSize: number = selectPagination(context).pageSize ?? 10;

            let props: Props = selectProps(context);

            /**
             * Get data from services
             */
            try {

                const [
                    groupFolderContents
                ] = await Promise.all([
                    getGroupFolders({
                        user: user,
                        groupId: groupId,
                        pagination: {
                            pageNumber: initialPageNumber,
                            pageSize: initialPageSize
                        }
                    })
                ]);

                props.folderContents = groupFolderContents.data ?? [];
                props.pagination = groupFolderContents.pagination ?? null;

                console.log(groupFolderContents);
            
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