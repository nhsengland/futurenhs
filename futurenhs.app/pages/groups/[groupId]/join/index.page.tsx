import { GetServerSideProps } from 'next';

import { routes } from '@constants/routes';
import { routeParams } from '@constants/routes';
import { layoutIds } from '@constants/routes';
import { actions as actionConstants } from '@constants/actions';
import { selectParam } from '@selectors/context';
import { withUser } from '@hofs/withUser';
import { withGroup } from '@hofs/withGroup';
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps';
import { postGroupMembership } from '@services/postGroupMembership';
import { GetServerSidePropsContext } from '@appTypes/next';

import { GroupUpdateTemplate } from '@components/_pageTemplates/GroupUpdateTemplate';
import { Props } from '@components/_pageTemplates/GroupUpdateTemplate/interfaces';

const routeId: string = 'not-required';
const props: Partial<Props> = {};

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withUser({
    props,
    getServerSideProps: withGroup({
        props,
        getServerSideProps: async (context: GetServerSidePropsContext) => {

            const groupId: string = selectParam(context, routeParams.GROUPID);

            props.layoutId = layoutIds.GROUP;
            props.tabId = 'index';

            /**
             * Return not found if user does not have valid action to join group
             */
            if(!props.actions?.includes(actionConstants.GROUPS_JOIN)){

                return {
                    notFound: true
                }

            }

            /**
             * Get data from services
             */
            try {

                await postGroupMembership({ groupId, user: props.user });

            } catch (error) {

                return handleSSRErrorProps({ props, error });

            }

            /**
             * Return data to page template
             */
            return {
                redirect: {
                    permanent: false,
                    destination: context.req.url.slice(0, context.req.url.lastIndexOf('/'))
                }
            }

        }
    })
});

/**
 * Export page template
 */
export default GroupUpdateTemplate;