import { GetServerSideProps } from 'next';

import { getServiceResponsesWithStatusCode } from '@helpers/services/getServiceResponsesWithStatusCode';
import { defaultGroupLogos } from '@constants/icons';
import { withAuth } from '@hofs/withAuth';
import { getGroup } from '@services/getGroup';
import { getGroupMember } from '@services/getGroupMember';
import { selectUser, selectGroupId, selectMemberId } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupMemberTemplate } from '@components/_pageTemplates/GroupMemberTemplate';
import { Props } from '@components/_pageTemplates/GroupMemberTemplate/interfaces';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth(async (context: GetServerSidePropsContext) => {

    const id: string = '4502d395-7c37-4e80-92b7-65886de858ef';

    /**
     * Get data from request context
     */
    const groupId: string = selectGroupId(context);
    const memberId: string = selectMemberId(context);
    const user: User = selectUser(context);

    /**
     * Create page data
     */
    const props: Props = {
        id: id,
        user: user,
        content: null,
        member: null,
        image: null,
        errors: null
    };

    /**
     * Get data from services
     */
    try {

        const [
            groupData,
            memberData
        ] = await Promise.all([
            getGroup({
                groupId: groupId
            }),
            getGroupMember({
                groupId: groupId,
                user: user,
                memberId: memberId
            })
        ]);

        if(getServiceResponsesWithStatusCode({
            serviceResponseList: [groupData, memberData],
            statusCode: 404
        }).length > 0){

            return {
                notFound: true
            }

        }

        props.content = groupData.data.content;
        props.image = groupData.data.image ?? defaultGroupLogos.small;
        props.member = memberData.data;
    
    } catch (error) {
        
        //props.errors = error;

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
export default GroupMemberTemplate;