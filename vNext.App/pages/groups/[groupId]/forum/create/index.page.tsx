import { GetServerSideProps } from 'next';

import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { selectCsrfToken, selectBody } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';

import { createDiscussionForm } from '@formConfigs/create-discussion';
import { GroupCreateDiscussionTemplate } from '@components/_pageTemplates/GroupCreateDiscussionTemplate';

const routeId: string = 'fcf3d540-9a55-418c-b317-a14146ae075f';

/**
 * Get props to inject into page on the initial server-side request
 */
 export const getServerSideProps: GetServerSideProps = withAuth({
    getServerSideProps: withGroup({
            routeId: routeId,
            getServerSideProps: async (context: GetServerSidePropsContext) => {

                const csrfToken: string = selectCsrfToken(context);
                const formPost: any = selectBody(context);

                let { props } = context;

                if(formPost){

                    return {
                        redirect: {
                            permanent: false,
                            destination: '/'
                        }
                    }

                }

                props.csrfToken = csrfToken;
                props.forms = {
                    [createDiscussionForm.id]: createDiscussionForm
                };

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
export default GroupCreateDiscussionTemplate;