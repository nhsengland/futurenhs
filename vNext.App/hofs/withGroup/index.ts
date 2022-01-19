import { GetServerSideProps } from 'next';

import { getServiceResponsesWithStatusCode } from '@helpers/services/getServiceResponsesWithStatusCode';
import { defaultGroupLogos } from '@constants/icons';
import { selectUser, selectGroupId } from '@selectors/context';
import { getGroup } from '@services/getGroup';
import { GetGroupService } from '@services/getGroup';
import { getGroupActions } from '@services/getGroupActions';
import { GetGroupActionsService } from '@services/getGroupActions';
import { GetServerSidePropsContext, HofConfig } from '@appTypes/next';
import { GroupPage } from '@appTypes/page';
import { User } from '@appTypes/user';

export const withGroup = (config: HofConfig, dependencies?: {
    getGroupService?: GetGroupService,
    getGroupActionsService?: GetGroupActionsService
}): GetServerSideProps => {

    const getGroupService = dependencies?.getGroupService ?? getGroup;
    const getGroupActionsService = dependencies?.getGroupService ?? getGroupActions;

    const { routeId, getServerSideProps } = config;

    return async (context: GetServerSidePropsContext): Promise<any> => {

        /**
         * Get data from request context
         */
        const groupId: string = selectGroupId(context);
        const user: User = selectUser(context);

        /**
         * Create page data
         */
        const props: Partial<GroupPage> = {
            id: routeId,
            user: user,
            content: null,
            image: null,
            actions: null
        };

        /**
         * Get data from services
         */
        try {

            const [
                groupData,
                actionsData
            ] = await Promise.all([
                getGroupService({
                    groupId: groupId
                }),
                getGroupActionsService({
                    groupId: groupId,
                    user: user
                })
            ]);

            if (getServiceResponsesWithStatusCode({
                serviceResponseList: [groupData],
                statusCode: 404
            }).length > 0) {

                return {
                    notFound: true
                }

            }

            props.content = groupData.data.content ?? null;
            props.image = groupData.data.image ?? defaultGroupLogos.small;
            (props as any).actions = actionsData.data ?? null;

        } catch (error) {

            console.log(error);

        }

        context.props = props;

        return await getServerSideProps(context);

    }

}
