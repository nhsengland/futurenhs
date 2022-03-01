import { GetServerSideProps } from 'next';

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps';
import { layoutIds } from '@constants/routes';
import { withUser } from '@hofs/withUser';
import { withGroup } from '@hofs/withGroup';
import { GetServerSidePropsContext } from '@appTypes/next';

import { GroupHomeTemplate } from '@components/_pageTemplates/GroupHomeTemplate';
import { Props } from '@components/_pageTemplates/GroupHomeTemplate/interfaces';

const routeId: string = '7a9bdd18-45ea-4976-9810-2fcb66242e27';
const props: Partial<Props> = {};

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withUser({
    props,
    getServerSideProps: withGroup({
        props,
        routeId,
        getServerSideProps: async (context: GetServerSidePropsContext) => {

            props.layoutId = layoutIds.GROUP;
            props.tabId = 'index';

            console.log(props);

            /**
             * Return data to page template
             */
            return handleSSRSuccessProps({ props });

        }
    })
});

/**
 * Export page template
 */
export default GroupHomeTemplate;