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

const routeId: string = '3c745d1d-9742-459a-a2bb-7af14c2f291c';
const isMember: boolean = true;

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
            const initialPageNumber: number = selectPagination(context).pageNumber ?? 1;
            const initialPageSize: number = selectPagination(context).pageSize ?? 10;
            const props: Props = selectProps(context);

            const pagination: Pagination = {
                pageNumber: initialPageNumber,
                pageSize: initialPageSize
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
