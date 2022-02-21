import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps';
import { withAuth } from '@hofs/withAuth';
import { withTextContent } from '@hofs/withTextContent';
import { getGroups } from '@services/getGroups';
import { selectUser, selectPagination } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupListingTemplate } from '@components/_pageTemplates/GroupListingTemplate';
import { Props } from '@components/_pageTemplates/GroupListingTemplate/interfaces';
import { Pagination } from '@appTypes/pagination';

const routeId: string = '3c745d1d-9742-459a-a2bb-7af14c2f291c';
const props: Partial<Props> = {};
const isMember: boolean = true;

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth({
    props,
    getServerSideProps: withTextContent({
        props,
        routeId,
        getServerSideProps: async (context: GetServerSidePropsContext) => {

            /**
             * Get data from request context
             */
            const user: User = selectUser(context);
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
            return handleSSRSuccessProps({ props });
    
        }
    })
});

/**
 * Export page template
 */
export default GroupListingTemplate;
