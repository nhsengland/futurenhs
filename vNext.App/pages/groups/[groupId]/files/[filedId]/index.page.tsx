import { GetServerSideProps } from 'next';

import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
//import { getGroupFiles } from '@services/getGroupFolders';
import { GetServerSidePropsContext } from '@appTypes/next';

import { GroupFileTemplate } from '@components/_pageTemplates/GroupFileTemplate';

const routeId: string = 'b74b9b6b-0462-4c2a-8859-51d0df17f68f';

/**
 * Get props to inject into page on the initial server-side request
 */
 export const getServerSideProps: GetServerSideProps = withAuth({
    getServerSideProps: withGroup({
            routeId: routeId,
            getServerSideProps: async (context: GetServerSidePropsContext) => {

                const { props } = context;

                /**
                 * Return data to page template
                 */
                return {
                    props: props
                }

            }
        })
});

/**
 * Export page template
 */
export default GroupFileTemplate;