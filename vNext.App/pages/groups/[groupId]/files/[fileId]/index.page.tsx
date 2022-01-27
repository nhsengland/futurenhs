import { GetServerSideProps } from 'next';

import { getServiceResponseErrors } from '@helpers/services/getServiceResponseErrors';
import { routeParams } from '@constants/routes';
import { getJsonSafeObject } from '@helpers/routing/getJsonSafeObject';
import { selectUser, selectParam, selectProps } from '@selectors/context';
import { withAuth } from '@hofs/withAuth';
import { withGroup } from '@hofs/withGroup';
import { getGroupFile } from '@services/getGroupFile';
import { GetServerSidePropsContext } from '@appTypes/next';
import { User } from '@appTypes/user';

import { GroupFileTemplate } from '@components/_pageTemplates/GroupFileTemplate';
import { Props } from '@components/_pageTemplates/GroupFileTemplate/interfaces';

const routeId: string = 'b74b9b6b-0462-4c2a-8859-51d0df17f68f';

/**
 * Get props to inject into page on the initial server-side request
 */
 export const getServerSideProps: GetServerSideProps = withAuth({
    getServerSideProps: withGroup({
            routeId: routeId,
            getServerSideProps: async (context: GetServerSidePropsContext) => {

                const user: User = selectUser(context);
                const groupId: string = selectParam(context, routeParams.GROUPID);
                const fileId: string = selectParam(context, routeParams.FILEID);

                let props: Props = selectProps(context);

                /**
                 * Get data from services
                 */
                try {

                    const [
                        groupFile
                    ] = await Promise.all([
                        getGroupFile({
                            user: user,
                            groupId: groupId,
                            fileId: fileId
                        })
                    ]);

                    if(getServiceResponseErrors({
                        serviceResponseList: [groupFile],
                        matcher: (code) => code === 404
                    }).length > 0){
    
                        return {
                            notFound: true
                        }
    
                    }

                    props.fileId = fileId;
                    props.file = groupFile.data;
                    props.errors = Object.assign(props.errors, groupFile.errors);

                } catch (error) {
                
                    props.errors = {
                        error: error.message
                    };
    
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
export default GroupFileTemplate;