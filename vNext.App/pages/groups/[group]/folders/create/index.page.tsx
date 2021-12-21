import { GetServerSideProps } from 'next';

import { withAuth } from '@hofs/withAuth';
import { getGroup } from '@services/getGroup';
import { selectUser, selectFolderId, selectCsrfToken } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupCreateFolderTemplate } from '@components/_pageTemplates/GroupCreateFolderTemplate';
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
    const folderId: string = selectFolderId(context);
    const csrfToken: string = selectCsrfToken(context);

    /**
     * Get data from services
     */
    const { data: { content } } = await getGroup({
        user: user,
        slug: slug,
        page: 'files'
    });

    /**
     * Return data to page template
     */
    return {
        props: {
            csrfToken: csrfToken,
            user: user,
            content: content,
            folderId: folderId,
        } as Props
    }

});

/**
 * Export page template
 */
export default GroupCreateFolderTemplate;