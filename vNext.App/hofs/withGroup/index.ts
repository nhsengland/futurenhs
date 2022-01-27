import { GetServerSideProps } from 'next';

import { routeParams } from '@constants/routes';
import { getServiceResponseErrors } from '@helpers/services/getServiceResponseErrors';
import { defaultGroupLogos } from '@constants/icons';
import { selectUser, selectParam } from '@selectors/context';
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
    const getGroupActionsService = dependencies?.getGroupActionsService ?? getGroupActions;

    const { routeId, getServerSideProps } = config;

    return async (context: GetServerSidePropsContext): Promise<any> => {

        /**
         * Get data from request context
         */
        const groupId: string = selectParam(context, routeParams.GROUPID);
        const user: User = selectUser(context);

        /**
         * Create page data
         */
        const props: Partial<GroupPage> = {
            id: routeId,
            user: user,
            text: null,
            image: null,
            actions: []
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

            if (getServiceResponseErrors({
                serviceResponseList: [groupData],
                matcher: (code) => Number(code) === 404
            }).length > 0) {

                return {
                    notFound: true
                }

            }

            if (getServiceResponseErrors({
                serviceResponseList: [groupData, actionsData],
                matcher: (code) => Number(code) >= 500
            }).length > 0) {

                return {
                    props: {
                        errors: [{
                            [500]: 'Server error'
                        }]
                    }
                }

            }

            props.groupId = groupId;
            props.text = groupData.data.text ?? null;
            props.image = groupData.data.image ?? defaultGroupLogos.small;
            props.actions = actionsData.data ?? null;

        } catch (error) {

            console.log(error);

        }

        context.props = props;

        return await getServerSideProps(context);

    }

}
