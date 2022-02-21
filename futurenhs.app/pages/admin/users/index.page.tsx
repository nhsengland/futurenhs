import { GetServerSideProps } from 'next';

import { actions } from '@constants/actions';
import { withAuth } from '@hofs/withAuth'
import { withTextContent } from '@hofs/withTextContent'
import { GetServerSidePropsContext } from '@appTypes/next';

import { AdminUsersTemplate } from '@components/_pageTemplates/AdminUsersTemplate';
import { Props } from '@components/_pageTemplates/AdminUsersTemplate/interfaces';

const routeId: string = '9e86c5cc-6836-4319-8d9d-b96249d4c909';
const props: Partial<Props> = {};

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth({
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
export default AdminUsersTemplate;
