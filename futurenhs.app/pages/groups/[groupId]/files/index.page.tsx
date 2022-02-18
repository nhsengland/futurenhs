import { GetServerSideProps } from 'next';

import { withAuth } from '@hofs/withAuth';
import { GetServerSidePropsContext } from '@appTypes/next';

const NoopTemplate = (props: any) => null;

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth({
    getServerSideProps: async (context: GetServerSidePropsContext) => {

        return {
            redirect: {
                permanent: false,
                destination: '/'
            }
        }  

    }
});

/**
 * Export page template
 */
export default NoopTemplate;