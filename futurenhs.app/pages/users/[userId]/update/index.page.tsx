import { GetServerSideProps } from 'next'

import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { layoutIds, routeParams } from '@constants/routes'
import { withUser } from '@hofs/withUser'
import { withTextContent } from '@hofs/withTextContent'
import { withRoutes } from '@hofs/withRoutes'
import {
    selectCsrfToken,
    selectFormData,
    selectMultiPartFormData,
    selectParam,
    selectRequestMethod,
    selectUser,
} from '@selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { getSiteUser } from '@services/getSiteUser'
import { getSiteUserRole } from '@services/getSiteUserRole'
import { formTypes } from '@constants/forms'
import { FormConfig, FormOptions } from '@appTypes/form'
import { setFormConfigOptions } from '@helpers/util/form'
import { Props } from '@components/_pageTemplates/SiteUserTemplate/interfaces'
import { requestMethods } from '@constants/fetch'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { putSiteUser } from '@services/putSiteUser'
import { FormErrors } from '@appTypes/form'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { User } from '@appTypes/user'
import { SiteUserUpdateTemplate } from '@components/_pageTemplates/SiteUserUpdateTemplate'
import { actions } from '@constants/actions'
import { putSiteUserRole } from '@services/putSiteUserRole'
import { getSiteUserRoles } from '@services/getSiteUserRoles'
import { selectForm } from '@selectors/forms'
import formConfigs from '@formConfigs/index'

const routeId: string = '9e86c5cc-6836-4319-8d9d-b96249d4c909'
const props: Partial<Props> = {}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = withUser({
    props,
    getServerSideProps: withRoutes({
        props,
        getServerSideProps: withTextContent({
            props,
            routeId,
            getServerSideProps: async (context: GetServerSidePropsContext) => {
                const targetUserId: string = selectParam(
                    context,
                    routeParams.USERID
                )
                const user: User = selectUser(context)

                const isAdmin = props.actions.includes(
                    actions.SITE_ADMIN_MEMBERS_EDIT
                )
                const hasEditPermissions = isAdmin || targetUserId === user.id

                if (!hasEditPermissions) {
                    return {
                        notFound: true,
                    }
                }

                const csrfToken: string = selectCsrfToken(context)
                const currentValues: any = selectFormData(context)
                const submission: any = selectMultiPartFormData(context)
                const requestMethod: requestMethods =
                    selectRequestMethod(context)

                props.layoutId = layoutIds.ADMIN
                props.forms = {
                    [formTypes.UPDATE_SITE_USER]: {},
                    [formTypes.UPDATE_SITE_USER_ROLE]: selectForm(
                        formConfigs,
                        formTypes.UPDATE_SITE_USER_ROLE
                    ),
                }

                const profileForm: FormConfig =
                    props.forms[formTypes.UPDATE_SITE_USER]

                const roleForm: FormConfig =
                    props.forms[formTypes.UPDATE_SITE_USER_ROLE]

                /**
                 * Get data from services
                 */
                try {
                    const [userData] = await Promise.all([
                        getSiteUser({ user, targetUserId, isForUpdate: true }),
                    ])

                    props.siteUser = userData.data
                    props.pageTitle = `${props.contentText.title} - ${
                        props.siteUser.firstName ?? ''
                    } ${props.siteUser.lastName ?? ''}`

                    const profileEtag = userData?.headers.get('etag')

                    props.etag = {
                        profileEtag: profileEtag,
                    }

                    profileForm.initialValues = {
                        firstName: props.siteUser.firstName,
                        lastName: props.siteUser.lastName,
                        pronouns: props.siteUser.pronouns,
                        id: props.siteUser.id,
                        imageId: props.siteUser.imageId,
                    }

                    if (isAdmin) {
                        /**
                         * Get role data from services if user is a platform admin
                         */
                        const [userRole, userRolesList] = await Promise.all([
                            getSiteUserRole({ user, targetUserId }),
                            getSiteUserRoles({ user }),
                        ])

                        const roleEtag = userRole?.headers.get('etag')
                        props.etag.roleEtag = roleEtag

                        /**
                         * Handle setting role options for multi-choice
                         */
                        const roleOptions: Array<FormOptions> =
                            userRolesList?.data?.map((role) => {
                                return {
                                    value: role.id,
                                    label: role.name,
                                }
                            })

                        const updatedRolesForm: FormConfig =
                            setFormConfigOptions(
                                roleForm,
                                0,
                                'newRoleId',
                                roleOptions
                            )

                        props.forms[formTypes.UPDATE_SITE_USER_ROLE] =
                            updatedRolesForm

                        updatedRolesForm.initialValues = {
                            newRoleId: userRole.data?.roleId,
                            currentRoleId: userRole.data?.roleId,
                        }

                        /**
                         * Handle server-side role form post
                         */
                        if (
                            submission &&
                            requestMethod === requestMethods.POST
                        ) {
                            if (
                                currentValues.body?.['_form-id'] ===
                                updatedRolesForm.id
                            ) {
                                updatedRolesForm.initialValues =
                                    currentValues.getAll()

                                const headers = getStandardServiceHeaders({
                                    csrfToken,
                                    etag: roleEtag,
                                })

                                await putSiteUserRole({
                                    headers,
                                    user,
                                    body: currentValues,
                                    targetUserId,
                                })
                            }
                        }
                    }

                    /**
                     * Handle server-side profile form post
                     */
                    if (submission && requestMethod === requestMethods.POST) {
                        if (
                            currentValues.body?.['_form-id'] ===
                            formTypes.UPDATE_SITE_USER
                        ) {
                            profileForm.initialValues = currentValues.getAll()

                            const headers = {
                                ...getStandardServiceHeaders({
                                    csrfToken,
                                    etag: profileEtag,
                                }),
                                ...submission.getHeaders(),
                            }

                            await putSiteUser({
                                headers,
                                user,
                                body: submission,
                                targetUserId,
                            })
                        }

                        return {
                            redirect: {
                                permanent: false,
                                destination: `${props.routes.usersRoot}/${targetUserId}`,
                            },
                        }
                    }
                } catch (error: any) {
                    const validationErrors: FormErrors =
                        getServiceErrorDataValidationErrors(error)

                    if (validationErrors) {
                        profileForm.errors = validationErrors
                    } else {
                        return handleSSRErrorProps({ props, error })
                    }
                }

                /**
                 * Return data to page template
                 */
                return handleSSRSuccessProps({ props, context })
            },
        }),
    }),
})

/**
 * Export page template
 */
export default SiteUserUpdateTemplate
