import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps';
import { layoutIds } from '@constants/routes';
import { routeParams } from '@constants/routes';
import { withUser } from '@hofs/withUser';
import { withConfig } from '@hofs/withConfig';
import { withGroup } from '@hofs/withGroup';
import { withTextContent } from '@hofs/withTextContent';
import { getGroupDiscussions } from '@services/getGroupDiscussions';
import { selectUser, selectPagination, selectParam } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupForumTemplate } from '@components/_pageTemplates/GroupForumTemplate';
import { Props } from '@components/_pageTemplates/GroupForumTemplate/interfaces';
import { Pagination } from '@appTypes/pagination';

const routeId: string = 'd7752e9e-4f47-41ec-bc07-70508d8dcd9b';
const props: Partial<Props> = {};

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withUser({
    props,
    getServerSideProps: withConfig({
        props,
        routeId,
        getServerSideProps: withGroup({
            props,
            getServerSideProps: withTextContent({
                props,
                routeId,
                getServerSideProps: async (context: GetServerSidePropsContext) => {

                    /**
                     * Get context data
                     */
                    const user: User = selectUser(context);
                    const groupId: string = selectParam(context, routeParams.GROUPID);
                    const pagination: Pagination = {
                        pageNumber: selectPagination(context).pageNumber ?? 1,
                        pageSize: selectPagination(context).pageSize ?? 5
                    };

                    props.layoutId = layoutIds.GROUP;
                    props.tabId = 'forum';
                
                    /**
                     * Get data from services
                     */
                    try {
                
                        const [groupDiscussions] = await Promise.all([getGroupDiscussions({ user, groupId, pagination })]);
                
                        props.discussionsList = groupDiscussions.data;
                        props.pagination = groupDiscussions.pagination;
                
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
export default GroupForumTemplate;
