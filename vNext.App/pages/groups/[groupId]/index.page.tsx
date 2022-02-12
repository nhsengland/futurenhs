import { GetServerSideProps } from 'next';

import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { selectProps } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';

import { GroupHomeTemplate } from '@components/_pageTemplates/GroupHomeTemplate';
import { Props } from '@components/_pageTemplates/GroupHomeTemplate/interfaces';

const routeId: string = '7a9bdd18-45ea-4976-9810-2fcb66242e27';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth({
    routeId: routeId,
    getServerSideProps: withGroup({
            routeId: routeId,
            getServerSideProps: async (context: GetServerSidePropsContext) => {

                const props: Props = selectProps(context);

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
export default GroupHomeTemplate;