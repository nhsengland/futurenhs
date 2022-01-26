import { GetServerSideProps } from 'next';

import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { routeParams } from '@constants/routes';
import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { getServiceResponsesWithStatusCode } from '@helpers/services/getServiceResponsesWithStatusCode';
import { getGroupDiscussion } from '@services/getGroupDiscussion';
import { selectUser, selectParam, selectProps } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupDiscussionTemplate } from '@components/_pageTemplates/GroupDiscussionTemplate';
import { Props } from '@components/_pageTemplates/GroupDiscussionTemplate/interfaces';

const routeId: string = 'f9658510-6950-43c4-beea-4ddeca277a5f';

/**
 * Get props to inject into page on the initial server-side request
 */
 export const getServerSideProps: GetServerSideProps = withAuth({
    getServerSideProps: withGroup({
        routeId: routeId,
        getServerSideProps: async (context: GetServerSidePropsContext) => {

            const user: User = selectUser(context);
            const groupId: string = selectParam(context, routeParams.GROUPID);
            const discussionId: string = selectParam(context, routeParams.DISCUSSIONID);

            let props: Props = selectProps(context);

            /**
             * Get data from services
             */
            try {

                const [
                    groupDiscussion
                ] = await Promise.all([
                    getGroupDiscussion({
                        user: user,
                        groupId: groupId,
                        discussionId: discussionId
                    })
                ]);

                if(getServiceResponsesWithStatusCode({
                    serviceResponseList: [groupDiscussion],
                    statusCode: 404
                }).length > 0){

                    return {
                        notFound: true
                    }

                }

                props.discussionId = discussionId;
                props.discussion = groupDiscussion.data;
                props.errors = Object.assign(props.errors, groupDiscussion.errors);
            
            } catch (error) {
                
                props.errors = {
                    error: error.message
                };

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
export default GroupDiscussionTemplate;
