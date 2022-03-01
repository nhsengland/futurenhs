import { GetServerSideProps } from 'next';

import { formTypes } from '@constants/forms';
import { actions } from '@constants/actions';
import { layoutIds } from '@constants/routes';
import { withUser } from '@hofs/withUser';
import { withGroup } from '@hofs/withGroup';
import { withForms } from '@hofs/withForms';
import { selectCsrfToken } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';

import { GroupUpdateTemplate } from '@components/_pageTemplates/GroupUpdateTemplate';
import { Props } from '@components/_pageTemplates/GroupUpdateTemplate/interfaces';

const routeId: string = '578dfcc6-857f-4eda-8779-1d9b110888c7';
const props: Partial<Props> = {};

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withUser({
    props,
    getServerSideProps: withGroup({
        props,
        getServerSideProps: withForms({
            props,
            routeId,
            getServerSideProps: async (context: GetServerSidePropsContext) => {

                const csrfToken: string = selectCsrfToken(context);

                props.forms[formTypes.UPDATE_GROUP].initialValues = {
                    'name': props.entityText.title,
                    'strapline': props.entityText.strapLine,
                    'themeId': [props.themeId]
                };
                props.layoutId = layoutIds.GROUP;
                props.tabId = 'index';

                /**
                 * Return data to page template
                 */
                return {
                    notFound: !props.actions.includes(actions.GROUPS_EDIT),
                    props: props
                }

            }
        })
    })
});

/**
 * Export page template
 */
export default GroupUpdateTemplate;