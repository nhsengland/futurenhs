import { GetServerSideProps } from 'next';

import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { withLogOut } from '@hofs/withLogOut';
import { getPageTextContent } from '@services/getPageTextContent';
import { selectLocale, selectUser } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { LoggedOutTemplate } from '@components/_pageTemplates/LoggedOutTemplate';
import { Props } from '@components/_pageTemplates/LoggedOutTemplate/interfaces';

const routeId: string = '9ecf0edb-3e8d-4c3b-b4e6-371e38ac0af4';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withLogOut({
    getServerSideProps: async (context: GetServerSidePropsContext) => {

        /**
         * Get data from request context
         */
        const user: User = selectUser(context);
        const locale: string = selectLocale(context);

        /**
         * Create page data
         */
        const props: Props = {
            id: routeId,
            user: user,
            text: null,
            logOutUrl: process.env.NEXT_PUBLIC_MVC_FORUM_LOGIN_URL
        };

        /**
         * Get data from services
         */
        try {

            const [
                pageTextContent
            ] = await Promise.all([
                getPageTextContent({
                    id: routeId,
                    locale: locale
                })
            ]);

            props.text = pageTextContent.data;
            props.errors = [...props.errors, ...pageTextContent.errors];
        
        } catch (error) {
            
            props.errors = [{
                error: error.message
            }];

        }

        /**
         * Return data to page template
         */
        return {
            props: getJsonSafeObject({
                object: props
            })
        }

    }

});

/**
 * Export page template
 */
export default LoggedOutTemplate;

