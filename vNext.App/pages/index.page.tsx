import { GetServerSideProps } from 'next';

import { HomeTemplate } from '@components/_pageTemplates/HomeTemplate';
import { withAuth } from '@hofs/withAuth';
import { selectUser, selectLocale } from '@selectors/context';
import { getPageContent } from '@services/getPageContent';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { Props } from '@components/_pageTemplates/HomeTemplate/interfaces';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth(async (context: GetServerSidePropsContext) => {

    const id: string = '749bd865-27b8-4af6-960b-3f0458f8e92f';

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
        content: null
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

});

/**
 * Export page template
 */
export default HomeTemplate;