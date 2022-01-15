import { GetServerSideProps } from 'next';

import { withAuth } from '@hofs/withAuth';
import { getGroup } from '@services/getGroup';
import { getGroupFiles } from '@services/getGroupFiles';
import { selectUser, selectPagination, selectFileId } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupFileTemplate } from '@components/_pageTemplates/GroupFileTemplate';
import { Props } from '@components/_pageTemplates/GroupFileTemplate/interfaces';

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
    const fileId: any = selectFileId(context);

    /**
     * Get data from services
     */
    const { data: { content } } = await getGroup({
        slug: slug
    });

    const { data: files, pagination } = await getGroupFiles({
        user: user,
        filters: {
            fileId: fileId
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
            fileId: fileId,
            file: files[0],
            pagination: pagination
        } as Props
    }

});

/**
 * Export page template
 */
export default GroupFileTemplate;