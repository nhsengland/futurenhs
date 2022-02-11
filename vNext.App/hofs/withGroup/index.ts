import { GetServerSideProps } from 'next';

import { routeParams } from '@constants/routes';
import { defaultGroupLogos } from '@constants/icons';
import { selectUser, selectParam, selectProps } from '@selectors/context';
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

        let props: GroupPage = selectProps(context);

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

            props.groupId = groupId;
            props.entityText = groupData.data.text ?? {};
            props.image = groupData.data.image ?? defaultGroupLogos.small;
            props.actions = actionsData.data ?? null;

        } catch (error) {

            if(error.name === 'ServiceError'){

                if(error.data?.status === 404){

                    return {
                        notFound: true
                    }

                } else {

                    return {
                        props: {
                            errors: [{
                                [error.data.status]: error.data.statusText
                            }]
                        }
                    }

                }

            }

        }

        context.props = props;

        return await getServerSideProps(context);

    }

}
