import { GetServerSideProps } from 'next';

import { withLogOut } from '@hofs/withLogOut';
import { getPageContent } from '@services/getPageContent';
import { selectLocale, selectUser } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { LoggedOutTemplate } from '@components/_pageTemplates/LoggedOutTemplate';
import { Props } from '@components/_pageTemplates/LoggedOutTemplate/interfaces';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withLogOut({
    getServerSideProps: async (context: GetServerSidePropsContext) => {

        const id: string = '9ecf0edb-3e8d-4c3b-b4e6-371e38ac0af4';

        /**
         * Get data from request context
         */
        const user: User = selectUser(context);
        const locale: string = selectLocale(context);

        /**
         * Create page data
         */
        const props: Props = {
            id: id,
            user: user,
            content: null,
            logOutUrl: process.env.NEXT_PUBLIC_MVC_FORUM_LOGIN_URL
        };

        /**
         * Get data from services
         */
        try {

            const [
                pageContent
            ] = await Promise.all([
                getPageContent({
                    id: id,
                    locale: locale
                })
            ]);

            props.content = pageContent.data;
        
        } catch (error) {
            
            props.errors = error;

        }

        /**
         * Return data to page template
         */
        return {
            props: props
        }

    }

});

/**
 * Export page template
 */
export default LoggedOutTemplate;

