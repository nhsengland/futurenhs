import { GetServerSideProps } from 'next';

import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { withAuth } from '@hofs/withAuth';
import { withTextContent } from '@hofs/withTextContent';
import { getGroups } from '@services/getGroups';
import { selectUser, selectPagination, selectProps } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupListingTemplate } from '@components/_pageTemplates/GroupListingTemplate';

const routeId: string = '3c745d1d-9742-459a-a2bb-7af14c2f291c';
const isGroupMember: boolean = true;

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
    
            let props: any = selectProps(context);
    
            /**
             * Get data from services
             */
            try {
    
                const [
                    groupsList,
                ] = await Promise.all([
                    getGroups({
                        user: user,
                        isMember: isGroupMember,
                        pagination: {
                            pageNumber: initialPageNumber,
                            pageSize: initialPageSize
                        }
                    })
                ]);
    
                props.isGroupMember = isGroupMember;
                props.groupsList = groupsList.data ?? [];
                props.pagination = groupsList.pagination;
            
            } catch (error) {

                if (error.name === 'ServiceError') {

                    props.errors = [{
                        [error.data.status]: error.data.statusText
                    }];

                }
    
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
