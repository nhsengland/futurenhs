import { GetServerSideProps } from 'next';

import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { GetServerSidePropsContext } from '@appTypes/next';

import { GroupForumTemplate } from '@components/_pageTemplates/GroupForumTemplate';

const routeId: string = 'd7752e9e-4f47-41ec-bc07-70508d8dcd9b';

/**
 * Get props to inject into page on the initial server-side request
 */
 export const getServerSideProps: GetServerSideProps = withAuth({
    routeId: routeId,
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
export default GroupForumTemplate;
