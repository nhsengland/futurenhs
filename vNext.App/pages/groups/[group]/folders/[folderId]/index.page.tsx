import { GetServerSideProps } from 'next';

import { withAuth } from '@hofs/withAuth';
import { getGroup } from '@services/getGroup';
import { getGroupFiles } from '@services/getGroupFiles';
import { selectUser, selectPagination, selectFolderId } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupFoldersTemplate } from '@components/_pageTemplates/GroupFoldersTemplate';
import { Props } from '@components/_pageTemplates/GroupFoldersTemplate/interfaces';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth(async (context: GetServerSidePropsContext) => {

    /**
     * Get data from request context
     */
    const pathElements: Array<string> = context.resolvedUrl.split('/');
    const slug: string = pathElements[pathElements.length - 3];
    const user: User = selectUser(context);
    const initialPageNumber: number = selectPagination(context).pageNumber ?? 1;
    const initialPageSize: number = selectPagination(context).pageSize ?? 30;
    const folderId: string = selectFolderId(context);

    /**
     * Get data from services
     */
    const { data: { content } } = await getGroup({
        slug: slug
    });

    const { data: files, pagination } = await getGroupFiles({
        user: user,
        filters: {
            folderId: folderId
        },
        pagination: {
            pageNumber: initialPageNumber,
            pageSize: initialPageSize
        }
    });

    /**
     * Return data to page template
     */
    return {
        props: {
            user: user,
            content: content,
            files: files,
            folderId: folderId,
            pagination: pagination
        } as Props
    }

});

/**
 * Export page template
 */
export default GroupFoldersTemplate;