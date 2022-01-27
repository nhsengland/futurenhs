import { GetServerSideProps } from 'next';

import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { routeParams } from '@constants/routes';
import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { getGroupMembers } from '@services/getGroupMembers';
import { getPendingGroupMembers } from '@services/getPendingGroupMembers';
import { getPageTextContent } from '@services/getPageTextContent';
import { selectUser, selectPagination, selectParam, selectLocale, selectProps } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupMemberListingTemplate } from '@components/_pageTemplates/GroupMemberListingTemplate';
import { Props } from '@components/_pageTemplates/GroupMemberListingTemplate/interfaces';

const routeId: string = '3d4a3b47-ba2c-43fa-97cf-90de93eeb4f8';

/**
 * Get props to inject into page on the initial server-side request
 */
 export const getServerSideProps: GetServerSideProps = withAuth({
    getServerSideProps: withGroup({
            routeId: routeId,
            getServerSideProps: async (context: GetServerSidePropsContext) => {

                const user: User = selectUser(context);
                const locale: string = selectLocale(context);
                const groupId: string = selectParam(context, routeParams.GROUPID);
                const initialPageNumber: number = selectPagination(context).pageNumber ?? 1;
                const initialPageSize: number = selectPagination(context).pageSize ?? 10;

                let props: Props = selectProps(context);

                /**
                 * Get data from services
                 */
                try {

                    const [
                        groupMembers,
                        groupPendingMembers,
                        pageTextContent
                    ] = await Promise.all([
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
                        }),
                        getPageTextContent({
                            id: routeId,
                            locale: locale
                        })
                    ]);

                    props.members = groupMembers.data;
                    props.pagination = groupMembers.pagination;
                    props.pendingMembers = groupPendingMembers.data;
                    props.text = Object.assign(props.text, pageTextContent.data);
                    props.errors = [...props.errors, ...pageTextContent.errors, ...groupMembers.errors, ...groupPendingMembers.errors];
                
                } catch (error) {
                    
                    props.errors = [{
                        error: error.message
                    }];

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
export default GroupMemberListingTemplate;