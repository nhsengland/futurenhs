import { GetServerSideProps } from 'next';

import { actions } from '@constants/actions';
import { withUser } from '@hofs/withUser'
import { withTextContent } from '@hofs/withTextContent'
import { GetServerSidePropsContext } from '@appTypes/next';

import { AdminGroupsTemplate } from '@components/_pageTemplates/AdminGroupsTemplate';
import { Props } from '@components/_pageTemplates/AdminGroupsTemplate/interfaces';

const routeId: string = '5943d34d-ee73-46da-bb9a-917ba8a2f421';
const props: Partial<Props> = {};

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withUser({
    props,
    getServerSideProps: withTextContent({
        props,
        routeId,
        getServerSideProps: async (context: GetServerSidePropsContext) => {

            /**
             * Return data to page template
             */
             return {
                notFound: !props.actions.includes(actions.SITE_ADMIN_VIEW),
                props: props
            }

        }
    })
});

/**
 * Export page template
 */
export default AdminGroupsTemplate;
