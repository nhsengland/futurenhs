import { GetServerSideProps } from 'next';

import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { routeParams } from '@constants/routes';
import { getServiceResponseErrors } from '@helpers/services/getServiceResponseErrors';
import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { getGroupMember } from '@services/getGroupMember';
import { getPageTextContent } from '@services/getPageTextContent';
import { selectUser, selectParam, selectLocale } from '@selectors/context';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupMemberTemplate } from '@components/_pageTemplates/GroupMemberTemplate';

const routeId: string = '4502d395-7c37-4e80-92b7-65886de858ef';

/**
 * Get props to inject into page on the initial server-side request
 */
 export const getServerSideProps: GetServerSideProps = withAuth({
    routeId: routeId,
    getServerSideProps: withGroup({
            routeId: routeId,
            getServerSideProps: async (context: GetServerSidePropsContext) => {

                const user: User = selectUser(context);
                const locale: string = selectLocale(context);
                const groupId: string = selectParam(context, routeParams.GROUPID);
                const memberId: string = selectParam(context, routeParams.MEMBERID);

                let { props } = context;

                /**
                 * Get data from services
                 */
                try {

                    const [
                        memberData,
                        pageTextContent
                    ] = await Promise.all([
                        getGroupMember({
                            groupId: groupId,
                            user: user,
                            memberId: memberId
                        }),
                        getPageTextContent({
                            id: routeId,
                            locale: locale
                        })
                    ]);

                    if(getServiceResponseErrors({
                        serviceResponseList: [memberData],
                        matcher: (code) => Number(code) === 404
                    }).length > 0){

                        return {
                            notFound: true
                        }

                    }

                    props.member = memberData.data;
                    props.text = Object.assign({}, props.text, pageTextContent.data ?? {});
                    props.errors = [...pageTextContent.errors, ...memberData.errors];
                
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
        })
});

/**
 * Export page template
 */
 export default GroupMemberTemplate;
