import { GetServerSideProps } from 'next';

import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps';
import { getRouteToParam2 } from '@helpers/routing/getRouteToParam';
import { GetServerSidePropsContext, HofConfig } from '@appTypes/next';

export const withRoutes = (config: HofConfig, dependencies?: {}): GetServerSideProps => {

    const { props, getServerSideProps } = config;

    return async (context: GetServerSidePropsContext): Promise<any> => {

        /**
         * Set up current routing data relative to context
         */
        try {

            const groupIndexRoute: string = getRouteToParam2({
                route: context.resolvedUrl,
                param: context.params?.groupId
            });
    
            props.routes = {
                groupRoot: groupIndexRoute,
                groupUpdate: groupIndexRoute ? `${groupIndexRoute}/update` : undefined,
                groupJoin: groupIndexRoute ? `${groupIndexRoute}/join` : undefined,
                groupLeave: groupIndexRoute ? `${groupIndexRoute}/leave` : undefined,
                groupForumRoot: groupIndexRoute ? `${groupIndexRoute}/forum` : undefined,
                groupFoldersRoot: groupIndexRoute ? `${groupIndexRoute}/folders` : undefined,
                groupFilesRoot: groupIndexRoute ? `${groupIndexRoute}/files` : undefined,
                groupMembersRoot: groupIndexRoute ? `${groupIndexRoute}/members` : undefined
            };

        } catch (error) {

            return handleSSRErrorProps({ props, error });

        }

        return await getServerSideProps(context);

    }

}
