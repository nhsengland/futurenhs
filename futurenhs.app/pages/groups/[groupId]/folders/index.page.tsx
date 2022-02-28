import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps';
import { routeParams } from '@constants/routes';
import { withUser } from '@hofs/withUser';
import { withGroup } from '@hofs/withGroup';
import { withTextContent } from '@hofs/withTextContent';
import { getGroupFolderContents } from '@services/getGroupFolderContents';
import { selectUser, selectPagination, selectParam } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';
import { Pagination } from '@appTypes/pagination';

import { GroupFolderContentsTemplate } from '@components/_pageTemplates/GroupFolderContentsTemplate';
import { Props } from '@components/_pageTemplates/GroupFolderContentsTemplate/interfaces';

const routeId: string = '8b74608e-e22d-4dd9-9501-1946ac27e133';
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
                const pagination: Pagination = {
                    pageNumber: selectPagination(context).pageNumber ?? 1,
                    pageSize: selectPagination(context).pageSize ?? 10
                };

                /**
                 * Get data from services
                 */
                try {

                    const [groupFolderContents] = await Promise.all([getGroupFolderContents({ user, groupId, pagination })]);

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