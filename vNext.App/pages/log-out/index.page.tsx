import { GetServerSideProps } from 'next';

import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { withLogOut } from '@hofs/withLogOut';
import { withTextContent } from '@hofs/withTextContent';
import { selectProps } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';

import { LoggedOutTemplate } from '@components/_pageTemplates/LoggedOutTemplate';
import { Props } from '@components/_pageTemplates/LoggedOutTemplate/interfaces';

const routeId: string = '9ecf0edb-3e8d-4c3b-b4e6-371e38ac0af4';

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withLogOut({
    getServerSideProps: withTextContent({
        routeId: routeId,
        getServerSideProps: async (context: GetServerSidePropsContext) => {

            const props: Props = selectProps(context);
    
            /**
             * Return data to page template
             */
            return {
                props: getJsonSafeObject({
                    object: props
                })
            }
    
        }
    
    })
});

/**
 * Export page template
 */
export default LoggedOutTemplate;
