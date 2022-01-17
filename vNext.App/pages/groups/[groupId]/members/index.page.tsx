import { GetServerSideProps } from 'next';

import { getServiceResponsesWithStatusCode } from '@helpers/services/getServiceResponsesWithStatusCode';
import { defaultGroupLogos } from '@constants/icons';
import { withAuth } from '@hofs/withAuth';
import { getGroup } from '@services/getGroup';
import { getGroupMembers } from '@services/getGroupMembers';
import { getPendingGroupMembers } from '@services/getPendingGroupMembers';
import { selectUser, selectPagination, selectGroupId } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupMemberListingTemplate } from '@components/_pageTemplates/GroupMemberListingTemplate';
import { Props } from '@components/_pageTemplates/GroupMemberListingTemplate/interfaces';  

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth(async (context: GetServerSidePropsContext) => {

    const id: string = '3d4a3b47-ba2c-43fa-97cf-90de93eeb4f8';

    /**
     * Get data from request context
     */
    const groupId: string = selectGroupId(context);
    const user: User = selectUser(context);
    const initialPageNumber: number = selectPagination(context).pageNumber ?? 1;
    const initialPageSize: number = selectPagination(context).pageSize ?? 10;

    /**
     * Create page data
     */
    const props: Props = {
        id: id,
        user: user,
        content: null,
        image: null,
        members: null,
        pendingMembers: null,
        pagination: null,
        errors: null
    };

    /**
     * Get data from services
     */
    try {

        const [
            groupData,
            groupMembers,
            groupPendingMembers
        ] = await Promise.all([
            getGroup({
                groupId: groupId
            }),
            getGroupMembers({
                user: user,
                groupId: groupId,
                pagination: {
                    pageNumber: initialPageNumber,
                    pageSize: initialPageSize
                }
            }),
            getPendingGroupMembers({
                user: user,
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
        props.members = groupMembers.data;
        props.pagination = groupMembers.pagination;
        props.pendingMembers = groupPendingMembers.data;
    
    } catch (error) {
        
        props.errors = error?.message ?? 'Error';

    }

    /**
     * Return data to page template
     */
    return {
        props: props
    };
    
});

/**
 * Export page template
 */
export default GroupMemberListingTemplate;