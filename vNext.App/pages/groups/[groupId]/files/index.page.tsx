import { GetServerSideProps } from 'next';

import { withAuth } from '@hofs/withAuth';
import { GetServerSidePropsContext } from '@appTypes/next';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth(async (context: GetServerSidePropsContext) => {

    return {
        redirect: {
            permanent: false,
            destination: '/'
        }
    }  

});

/**
 * Export page template
 */
export default (props: any) => null;