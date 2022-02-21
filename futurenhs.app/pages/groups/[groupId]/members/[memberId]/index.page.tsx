import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps';
import { routeParams } from '@constants/routes';
import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { withTextContent } from '@hofs/withTextContent';
import { getGroupMember } from '@services/getGroupMember';
import { selectUser, selectParam } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupMemberTemplate } from '@components/_pageTemplates/GroupMemberTemplate';
import { Props } from '@components/_pageTemplates/GroupMemberTemplate/interfaces';

const routeId: string = '4502d395-7c37-4e80-92b7-65886de858ef';
const props: Partial<Props> = {};

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth({
    props,
    getServerSideProps: withGroup({
        props,
        routeId,
        getServerSideProps: withTextContent({
            props,
            routeId,
            getServerSideProps: async (context: GetServerSidePropsContext) => {

                const user: User = selectUser(context);
                const groupId: string = selectParam(context, routeParams.GROUPID);
                const memberId: string = selectParam(context, routeParams.MEMBERID);

                /**
                 * Get data from services
                 */
                try {

                    const [memberData] = await Promise.all([getGroupMember({ groupId, user, memberId})]);

                    props.member = memberData.data;

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
});

/**
 * Export page template
 */
export default GroupMemberTemplate;
