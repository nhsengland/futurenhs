import { GetServerSideProps } from 'next';

import { getServiceResponsesWithStatusCode } from '@helpers/services/getServiceResponsesWithStatusCode';
import { defaultGroupLogos } from '@constants/icons';
import { withAuth } from '@hofs/withAuth';
import { getGroup } from '@services/getGroup';
import { GetServerSidePropsContext } from '@appTypes/next';
import { selectUser, selectGroupId } from '@selectors/context';
import { User } from '@appTypes/user';

import { GroupForumTemplate } from '@components/_pageTemplates/GroupForumTemplate';
import { Props } from '@components/_pageTemplates/GroupForumTemplate/interfaces';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth(async (context: GetServerSidePropsContext) => {

    const id: string = 'd7752e9e-4f47-41ec-bc07-70508d8dcd9b';

    /**
     * Get data from request context
     */
    const groupId: string = selectGroupId(context);
    const user: User = selectUser(context);

    /**
     * Create page data
     */
    const props: Props = {
        id: id,
        user: user,
        content: null,
        image: null,
        discussionsList: []
    };

    /**
     * Get data from services
     */
    try {

        const [
            groupData
        ] = await Promise.all([
            getGroup({
                groupId: groupId
            })
        ]);

        if(getServiceResponsesWithStatusCode({
            serviceResponseList: [groupData],
            statusCode: 404
        }).length > 0){

            return {
                notFound: true
            }

        }

        props.content = groupData.data.content;
        props.image = groupData.data.image ?? defaultGroupLogos.small;
    
    } catch (error) {
        
        props.errors = error;

    }

    /**
     * Return data to page template
     */
    return {
        props: props
    }

});

/**
 * Export page template
 */
export default GroupForumTemplate;
