import { GetServerSideProps } from 'next';

import { getServiceResponsesWithStatusCode } from '@helpers/services/getServiceResponsesWithStatusCode';
import { defaultGroupLogos } from '@constants/icons';
import { withAuth } from '@hofs/withAuth';
import { getGroup } from '@services/getGroup';
import { selectUser, selectFolderId, selectCsrfToken, selectGroupId } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupCreateFolderTemplate } from '@components/_pageTemplates/GroupCreateFolderTemplate';
import { Props } from '@components/_pageTemplates/GroupCreateFolderTemplate/interfaces';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth(async (context: GetServerSidePropsContext) => {

    const id: string = 'c1bc7b37-762f-4ed8-aed2-79fcd0e5d5d2';

    /**
     * Get data from request context
     */
    const groupId: string = selectGroupId(context);
    const user: User = selectUser(context);
    const folderId: string = selectFolderId(context);
    const csrfToken: string = selectCsrfToken(context);

    /**
     * Create page data
     */
    const props: Props = {
        id: id,
        csrfToken: csrfToken,
        user: user,
        image: null,
        content: null,
        folderId: folderId
    };

    /**
     * Get data from services
     */
    try {

        const [
            groupData,
        ] = await Promise.all([
            getGroup({
                groupId: groupId
            })
        ]);

        if(getServiceResponsesWithStatusCode({
            serviceResponseList: [groupData],
            statusCode: 404
        }).length > 0){

            return {
                notFound: true
            }

        }

        props.content = groupData.data.content;
        props.image = groupData.data.image ?? defaultGroupLogos.small;

    } catch (error) {
            
        props.errors = error;

    }

    /**
     * Return data to page template
     */
    return {
        props: props
    }

});

/**
 * Export page template
 */
export default GroupCreateFolderTemplate;