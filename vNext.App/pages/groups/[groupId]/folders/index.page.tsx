import { GetServerSideProps } from 'next';

import { getServiceResponsesWithStatusCode } from '@helpers/services/getServiceResponsesWithStatusCode';
import { defaultGroupLogos } from '@constants/icons';
import { withAuth } from '@hofs/withAuth';
import { getGroup } from '@services/getGroup';
import { getGroupFolders } from '@services/getGroupFolders';
import { selectUser, selectPagination, selectFolderId, selectGroupId } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupFoldersTemplate } from '@components/_pageTemplates/GroupFoldersTemplate';
import { Props } from '@components/_pageTemplates/GroupFoldersTemplate/interfaces';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth(async (context: GetServerSidePropsContext) => {

    const id: string = '8b74608e-e22d-4dd9-9501-1946ac27e133';

    /**
     * Get data from request context
     */
    const groupId: string = selectGroupId(context);
    const folderId: string = selectFolderId(context);
    const user: User = selectUser(context);
    const initialPageNumber: number = selectPagination(context).pageNumber ?? 1;
    const initialPageSize: number = selectPagination(context).pageSize ?? 30;

    /**
     * Create page data
     */
    const props: Props = {
        id: id,
        user: user,
        groupId: groupId,
        content: null,
        image: null,
        folderId: folderId,
        folder: null,
        files: [],
        pagination: null,
        errors: null
    };

    /**
     * Get data from services
     */
    try {

        const [
            groupData,
            groupFiles,
        ] = await Promise.all([
            getGroup({
                groupId: groupId
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

        if(getServiceResponsesWithStatusCode({
            serviceResponseList: [groupData, groupFiles],
            statusCode: 404
        }).length > 0){

            return {
                notFound: true
            }

        }

        props.content = groupData.data.content;
        props.image = groupData.data.image ?? defaultGroupLogos.small;
        props.files = groupFiles.data;
        props.pagination = groupFiles.pagination;
    
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
export default GroupFoldersTemplate;