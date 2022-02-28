import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps';
import { routeParams } from '@constants/routes';
import { withUser } from '@hofs/withUser';
import { withGroup } from '@hofs/withGroup';
import { withTextContent } from '@hofs/withTextContent';
import { getGroupFolder } from '@services/getGroupFolder';
import { getGroupFolderContents } from '@services/getGroupFolderContents';
import { selectUser, selectPagination, selectParam } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';
import { Pagination } from '@appTypes/pagination';

import { GroupFolderContentsTemplate } from '@components/_pageTemplates/GroupFolderContentsTemplate';
import { Props } from '@components/_pageTemplates/GroupFolderContentsTemplate/interfaces';

const routeId: string = '3ea9a707-4686-4129-a9fc-9041a6d5ae6e';
const props: Partial<Props> = {};

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withUser({
    props,
    getServerSideProps: withGroup({
        props,
        routeId,
        getServerSideProps: withTextContent({
            props,
            routeId,
            getServerSideProps: async (context: GetServerSidePropsContext) => {

                const user: User = selectUser(context);
                const groupId: string = selectParam(context, routeParams.GROUPID);
                const folderId: string = selectParam(context, routeParams.FOLDERID);
                const pagination: Pagination = {
                    pageNumber: selectPagination(context).pageNumber ?? 1,
                    pageSize: selectPagination(context).pageSize ?? 10
                };

                /**
                 * Get data from services
                 */
                try {

                    const [groupFolder, groupFolderContents] = await Promise.all([
                        getGroupFolder({ user, groupId, folderId }),
                        getGroupFolderContents({ user, groupId, folderId, pagination })
                    ]);

                    props.folderId = folderId;
                    props.folder = groupFolder.data;
                    props.folderContents = groupFolderContents.data ?? [];
                    props.pagination = groupFolderContents.pagination;

                } catch (error) {

                    return handleSSRErrorProps({ props, error });

                }

                /**
                 * Return data to page template
                 */
                return handleSSRSuccessProps({ props });

            }
        })
    })
});

/**
 * Export page template
 */
export default GroupFolderContentsTemplate;