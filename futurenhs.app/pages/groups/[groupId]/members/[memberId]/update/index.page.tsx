import { GetServerSideProps } from 'next'

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { routeParams } from '@constants/routes'
import { formTypes } from '@constants/forms'
import { layoutIds, groupTabIds } from '@constants/routes'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withGroup } from '@hofs/withGroup'
import { withForms } from '@hofs/withForms'
import { withTextContent } from '@hofs/withTextContent'
import { getGroupMember } from '@services/getGroupMember'
import { getGroupRoles } from '@services/getGroupRoles'
import { selectUser, selectParam, selectCsrfToken, selectFormData, selectRequestMethod } from '@selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'

import { GroupMemberUpdateTemplate } from '@components/_pageTemplates/GroupMemberUpdateTemplate'
import { Props } from '@components/_pageTemplates/GroupMemberTemplate/interfaces'
import { actions } from '@constants/actions'
import { FormOptions } from '@appTypes/form'
import { checkMatchingFormType, setFormConfigOptions } from '@helpers/util/form'
import { requestMethods } from '@constants/fetch'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { putGroupMemberRole } from '@services/putGroupMemberRole'
import { deleteGroupMember } from '@services/deleteGroupMember'

const routeId: string = '4502d395-7c37-4e80-92b7-65886de858ef'
const props: Partial<Props> = {}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withUser({
    props,
    getServerSideProps: withRoutes({
        props,
        getServerSideProps: withGroup({
            props,
            getServerSideProps: withForms({
                props,
                getServerSideProps: withTextContent({
                    props,
                    routeId,
                    getServerSideProps: async (
                        context: GetServerSidePropsContext
                    ) => {
                        
                        const user: User = selectUser(context)
                        const csrfToken: string = selectCsrfToken(context)
                        const currentValues: any = selectFormData(context)
                        const requestMethod: requestMethods = selectRequestMethod(context)

                        if (!props.actions.includes(actions.GROUPS_MEMBERS_EDIT)) {
                            return {
                                notFound: true,
                            }
                        }

                        const groupId: string = selectParam(
                            context,
                            routeParams.GROUPID
                        )
                        const memberId: string = selectParam(
                            context,
                            routeParams.MEMBERID
                        )

                        const form: any =
                            props.forms[formTypes.UPDATE_GROUP_MEMBER]

                        /**
                         * Get data from services
                         */
                        try {
                            const [memberData, groupRoles] = await Promise.all([
                                getGroupMember({ groupId, user, memberId, isForEdit: true }),
                                getGroupRoles({ groupId, user })
                            ])
                            const etag = memberData.headers?.get('etag');
                            props.etag = etag

                            props.member = memberData.data
                            props.layoutId = layoutIds.GROUP
                            props.tabId = groupTabIds.MEMBERS
                            props.pageTitle = `${props.entityText.title} - ${props.member.firstName ?? ''
                                } ${props.member.lastName ?? ''} - Edit`

                            /**
                             * Handle setting role options for multi-choice 
                             */
                            const roleOptions: Array<FormOptions> = groupRoles?.data?.map((role) => {
                                return {
                                    value: role.id,
                                    label: role.name
                                }
                            })

                            const updatedRolesForm = setFormConfigOptions(form, 0, 'groupUserRoleId', roleOptions);
                            const usersCurrentRole = groupRoles.data?.find((role) => role.name === props.member?.role)

                            props.forms[formTypes.UPDATE_GROUP_MEMBER] = updatedRolesForm;

                            updatedRolesForm.initialValues = {
                                groupUserRoleId: usersCurrentRole?.id,
                            }



                            /**
                             * Handle server-side form post
                             */
                            if (
                                currentValues &&
                                requestMethod === requestMethods.POST
                            ) {

                                const headers =
                                getStandardServiceHeaders({
                                    csrfToken,
                                    etag,
                                });

                                const isRoleForm = checkMatchingFormType(currentValues, updatedRolesForm.id)
                                const isDeleteForm = checkMatchingFormType(currentValues, formTypes.DELETE_GROUP_MEMBER)

                                if (isRoleForm) {

                                    updatedRolesForm.initialValues = currentValues.getAll();

                                    await putGroupMemberRole({
                                        headers,
                                        user,
                                        body: currentValues,
                                        groupId,
                                        memberId
                                    })
                                }

                                if (isDeleteForm) {

                                    await deleteGroupMember({groupId, groupUserId: memberId, headers, user})

                                }

                                return {
                                    redirect: {
                                        permanent: false,
                                        destination: `${props.routes.groupMembersRoot}`,
                                    },
                                }
                            }

                        
                        } catch(error) {
                        return handleSSRErrorProps({ props, error })
                    }

                        /**
                         * Return data to page template
                         */
                        return handleSSRSuccessProps({ props })
                },
                }),
        }),
    }),
}),
})

/**
 * Export page template
 */
export default GroupMemberUpdateTemplate