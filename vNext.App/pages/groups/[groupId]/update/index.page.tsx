import { GetServerSideProps } from 'next';

import { actions } from '@constants/actions';
import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { selectCsrfToken, selectProps } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';

import { GroupUpdateTemplate } from '@components/_pageTemplates/GroupUpdateTemplate';
import { Props } from '@components/_pageTemplates/GroupUpdateTemplate/interfaces';

const routeId: string = '578dfcc6-857f-4eda-8779-1d9b110888c7';

/**
 * Get props to inject into page on the initial server-side request
 */
 export const getServerSideProps: GetServerSideProps = withAuth({
    getServerSideProps: withGroup({
            routeId: routeId,
            getServerSideProps: async (context: GetServerSidePropsContext) => {

                const csrfToken: string = selectCsrfToken(context);

                let props: Props = selectProps(context);

                props.csrfToken = csrfToken;

                /**
                 * Return data to page template
                 */
                return {
                    notFound: !props.actions.includes(actions.GROUPS_EDIT),
                    props: props
                }

            }
        })
});

/**
 * Export page template
 */
export default GroupUpdateTemplate;