import { GetServerSideProps } from 'next';

import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { routeParams } from '@constants/routes';
import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { getGroupFolderContents } from '@services/getGroupFolderContents';
import { selectUser, selectPagination, selectParam, selectProps } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupFolderContentsTemplate } from '@components/_pageTemplates/GroupFolderContentsTemplate';
import { Props } from '@components/_pageTemplates/GroupFolderContentsTemplate/interfaces';

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
                    getGroupFolderContents({
                        user: user,
                        groupId: groupId,
                        pagination: {
                            pageNumber: initialPageNumber,
                            pageSize: initialPageSize
                        }
                    })
                ]);

                props.folderContents = groupFolderContents.data ?? [];
                props.pagination = groupFolderContents.pagination;
                props.errors = Object.assign(props.errors, groupFolderContents.errors);
            
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