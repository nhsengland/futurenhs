import { GetServerSideProps } from 'next';

import { defaultGroupLogos } from '@constants/icons';
import { withAuth } from '@hofs/withAuth';
import { getGroup } from '@services/getGroup';
import { selectUser } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupHomeTemplate } from '@components/_pageTemplates/GroupHomeTemplate';
import { Props } from '@components/_pageTemplates/GroupHomeTemplate/interfaces';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withAuth(async (context: GetServerSidePropsContext) => {

    const id: string = '7a9bdd18-45ea-4976-9810-2fcb66242e27';

    /**
     * Get data from request context
     */
    const slug: string = context.resolvedUrl.substring(context.resolvedUrl.lastIndexOf('/') + 1).split('?')[0];
    const user: User = selectUser(context);

    /**
     * Create page data
     */
    const props: Props = {
        id: id,
        user: user,
        content: null,
        image: null
    };

    /**
     * Get data from services
     */
    try {

        const [
            groupData
        ] = await Promise.all([
            getGroup({
                slug: slug
            })
        ]);

        props.content = groupData.data.content;
        props.image = groupData.data.image ?? defaultGroupLogos.small;
    
    } catch (error) {

        console.log(error);
        
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
export default GroupHomeTemplate;