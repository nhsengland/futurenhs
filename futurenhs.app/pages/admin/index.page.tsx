import { GetServerSideProps } from 'next';

import { actions } from '@constants/actions';
import { withAuth } from '@hofs/withAuth';
import { GetServerSidePropsContext } from '@appTypes/next';

const NoopTemplate = (props: any) => null;
const props: Partial<any> = {};

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth({
    props,
    getServerSideProps: async (context: GetServerSidePropsContext) => {

        return {
            notFound: !props.actions.includes(actions.SITE_ADMIN_VIEW),
            redirect: {
                permanent: false,
                destination: '/admin/users'
            }
        }  

    }
});

/**
 * Export page template
 */
export default NoopTemplate;