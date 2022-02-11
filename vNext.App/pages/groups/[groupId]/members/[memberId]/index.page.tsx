import { GetServerSideProps } from 'next';

import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { routeParams } from '@constants/routes';
import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { withTextContent } from '@hofs/withTextContent';
import { getGroupMember } from '@services/getGroupMember';
import { selectUser, selectParam } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupMemberTemplate } from '@components/_pageTemplates/GroupMemberTemplate';

const routeId: string = '4502d395-7c37-4e80-92b7-65886de858ef';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth({
    routeId: routeId,
    getServerSideProps: withGroup({
        routeId: routeId,
        getServerSideProps: withTextContent({
            routeId: routeId,
            getServerSideProps: async (context: GetServerSidePropsContext) => {

                const user: User = selectUser(context);
                const groupId: string = selectParam(context, routeParams.GROUPID);
                const memberId: string = selectParam(context, routeParams.MEMBERID);

                let { props } = context;

                /**
                 * Get data from services
                 */
                try {

                    const [
                        memberData
                    ] = await Promise.all([
                        getGroupMember({
                            groupId: groupId,
                            user: user,
                            memberId: memberId
                        })
                    ]);

                    props.member = memberData.data;

                } catch (error) {

                    if (error.name === 'ServiceError') {

                        if(error.data.status == 404){

                            return {
                                notFound: true
                            }

                        }

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
    })
});

/**
 * Export page template
 */
export default GroupMemberTemplate;
