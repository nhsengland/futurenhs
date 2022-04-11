import { GetServerSideProps } from 'next'

import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { requestMethods } from '@constants/fetch'
import { actions as actionConstants } from '@constants/actions'
import { layoutIds } from '@constants/routes'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withForms } from '@hofs/withForms'
import {
    selectFormData,
    selectCsrfToken,
    selectUser,
    selectRequestMethod,
} from '@selectors/context'
import { postSiteUserInvite } from '@services/postSiteUserInvite'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { FormErrors } from '@appTypes/form'

import { inviteUserForm } from '@formConfigs/invite-user'
import { AdminUsersInviteTemplate } from '@components/_pageTemplates/AdminUsersInviteTemplate'
import { Props } from '@components/_pageTemplates/GroupCreateDiscussionTemplate/interfaces'
import { withTextContent } from '@hofs/withTextContent'
import { ServerSideFormData } from '@helpers/util/form'

const routeId: string = '1324ce48-a906-4195-86f6-64c7d33c4191'
const props: Partial<Props> = {}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withUser({
    props,
    getServerSideProps: withRoutes({
        props,
        getServerSideProps: withForms({
            props,
            routeId,
            getServerSideProps: withTextContent({
                props,
                routeId,
                getServerSideProps: async (
                    context: GetServerSidePropsContext
                ) => {
                    const user: User = selectUser(context)
                    const csrfToken: string = selectCsrfToken(context)
                    const formData: ServerSideFormData = selectFormData(context)
                    const requestMethod: requestMethods =
                        selectRequestMethod(context)

                    const form: any = props.forms[inviteUserForm.id]

                    props.layoutId = layoutIds.ADMIN

                    /**
                     * Return page not found if user doesn't have permissions to invite a user
                     */
                    if (
                        !props.actions?.includes(
                            actionConstants.SITE_ADMIN_MEMBERS_ADD
                        )
                    ) {
                        return {
                            notFound: true,
                        }
                    }

                    /**
                     * Handle server-side form post
                     */
                    if (formData && requestMethod === requestMethods.POST) {
                        form.initialValues = formData

                        try {
                            const headers: any = getStandardServiceHeaders({
                                csrfToken,
                            })

                            await postSiteUserInvite({
                                user,
                                headers,
                                body: formData,
                            })

                            return {
                                props: props,
                            }
                        } catch (error) {
                            const validationErrors: FormErrors =
                                getServiceErrorDataValidationErrors(error)

                            if (validationErrors) {
                                form.errors = validationErrors
                            } else {
                                return handleSSRErrorProps({ props, error })
                            }
                        }
                    }

                    /**
                     * Return data to page template
                     */
                    return handleSSRSuccessProps({ props })
                },
            }),
        }),
    }),
})

/**
 * Export page template
 */
export default AdminUsersInviteTemplate
