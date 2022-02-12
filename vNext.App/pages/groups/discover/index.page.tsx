import { GetServerSideProps } from 'next';

import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps';
import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { withAuth } from '@hofs/withAuth';
import { withTextContent } from '@hofs/withTextContent';
import { getGroups } from '@services/getGroups';
import { selectUser, selectPagination, selectProps } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupListingTemplate } from '@components/_pageTemplates/GroupListingTemplate';
import { Props } from '@components/_pageTemplates/GroupListingTemplate/interfaces';
import { Pagination } from '@appTypes/pagination';

const routeId: string = '8190d347-e29a-4577-baa8-446bcae428d9';
const isMember: boolean = false;

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth({
    getServerSideProps: withTextContent({
        routeId: routeId,
        getServerSideProps: async (context: GetServerSidePropsContext) => {

            /**
             * Get data from request context
             */
            const user: User = selectUser(context);
            const props: Props = selectProps(context);
            const pagination: Pagination = {
                pageNumber: selectPagination(context).pageNumber ?? 1,
                pageSize: selectPagination(context).pageSize ?? 10
            };
    
            props.isGroupMember = isMember;

            /**
             * Get data from services
             */
            try {
    
                const [groupsList] = await Promise.all([getGroups({ user, isMember, pagination })]);
    
                props.groupsList = groupsList.data ?? [];
                props.pagination = groupsList.pagination;
            
            } catch (error) {

                return handleSSRErrorProps({ props, error });
    
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
export default GroupListingTemplate;
