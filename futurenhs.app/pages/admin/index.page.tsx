import { GetServerSideProps } from 'next';

import { actions } from '@constants/actions';
import { withAuth } from '@hofs/withAuth';
import { withTextContent } from '@hofs/withTextContent';
import { GetServerSidePropsContext } from '@appTypes/next';

import { AdminHomeTemplate } from '@components/_pageTemplates/AdminHomeTemplate';
import { Props } from '@components/_pageTemplates/AdminHomeTemplate/interfaces';

const props: Partial<Props> = {};
const routeId: string = '439794f2-9c58-4b6f-9fe8-d77a841e3055';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth({
    props,
    getServerSideProps: withTextContent({
        routeId,
        props,
        getServerSideProps: async (context: GetServerSidePropsContext) => {

            return {
                props,
                notFound: !props.actions.includes(actions.SITE_ADMIN_VIEW),
            }  
    
        }
    })
});

/**
 * Export page template
 */
export default AdminHomeTemplate;