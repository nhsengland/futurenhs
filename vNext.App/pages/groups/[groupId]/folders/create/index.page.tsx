import { GetServerSideProps } from 'next';

import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { selectCsrfToken } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';

import { GroupCreateFolderTemplate } from '@components/_pageTemplates/GroupCreateFolderTemplate';

const routeId: string = 'c1bc7b37-762f-4ed8-aed2-79fcd0e5d5d2';

/**
 * Get props to inject into page on the initial server-side request
 */
 export const getServerSideProps: GetServerSideProps = withAuth({
    getServerSideProps: withGroup({
            routeId: routeId,
            getServerSideProps: async (context: GetServerSidePropsContext) => {

                let { props } = context;

                const csrfToken: string = selectCsrfToken(context);

                props.csrfToken = csrfToken;

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
export default GroupCreateFolderTemplate;