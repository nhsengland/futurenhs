import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps';
import { routeParams } from '@constants/routes';
import { layoutIds, groupTabIds } from '@constants/routes';
import { withUser } from '@hofs/withUser';
import { withRoutes } from '@hofs/withRoutes';
import { withGroup } from '@hofs/withGroup';
import { withTextContent } from '@hofs/withTextContent';
import { getGroupMembers } from '@services/getGroupMembers';
import { getPendingGroupMembers } from '@services/getPendingGroupMembers';
import { selectUser, selectPagination, selectParam } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { Pagination } from '@appTypes/pagination';
import { User } from '@appTypes/user';

import { GroupMemberListingTemplate } from '@components/_pageTemplates/GroupMemberListingTemplate';
import { Props } from '@components/_pageTemplates/GroupMemberListingTemplate/interfaces';

const routeId: string = '3d4a3b47-ba2c-43fa-97cf-90de93eeb4f8';
const props: Partial<Props> = {};

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withUser({
    props,
    getServerSideProps: withRoutes({
        props,
        getServerSideProps: withGroup({
            props,
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
    
                    props.layoutId = layoutIds.GROUP;
                    props.tabId = groupTabIds.MEMBERS;
                    props.pageTitle = `${props.entityText.title} - ${props.contentText.subTitle}`
    
                    /**
                     * Get data from services
                     */
                    try {
    
                        const [groupMembers, groupPendingMembers] = await Promise.all([
                            getGroupMembers({ user, groupId, pagination }),
                            getPendingGroupMembers({ user, groupId })
                        ]);
    
                        props.members = groupMembers.data;
                        props.pagination = groupMembers.pagination;
                        props.pendingMembers = groupPendingMembers.data;
                        
    
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
    })
});

/**
 * Export page template
 */
export default GroupMemberListingTemplate;