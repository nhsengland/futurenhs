import { GetServerSideProps } from 'next'
import { useRef, useState } from 'react'
import { useRouter } from 'next/router'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
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
import { selectPageProps } from '@selectors/context'
import { formTypes } from '@constants/forms'
import { FormConfig, FormOptions } from '@appTypes/form'
import { setFormConfigOptions } from '@helpers/util/form'
import { requestMethods } from '@constants/fetch'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { putSiteUser } from '@services/putSiteUser'
import { FormErrors } from '@appTypes/form'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { User } from '@appTypes/user'
import { actions } from '@constants/actions'
import { putSiteUserRole } from '@services/putSiteUserRole'
import { getSiteUserRoles } from '@services/getSiteUserRoles'
import { selectForm, selectFormErrors } from '@selectors/forms'
import formConfigs from '@formConfigs/index'
import { actions as actionsConstants } from '@constants/actions'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { Avatar } from '@components/Avatar'
import { initials } from '@helpers/formatters/initials'
import { Form } from '@components/Form'
import { ErrorSummary } from '@components/ErrorSummary'
import { getGenericFormError } from '@helpers/util/form'
import { useFormConfig } from '@hooks/useForm'
import { GenericPageTextContent } from '@appTypes/content'
import { Page } from '@appTypes/page'

declare interface ContentText extends GenericPageTextContent {
    firstNameLabel: string
    lastNameLabel: string
    pronounsLabel: string
    emailLabel: string
    editHeading?: string
    editButtonLabel?: string
    editRoleHeading?: string
}

export interface Props extends Page {
    siteUser: any
    contentText: ContentText
}

export const UserUpdatePage: (props: Props) => JSX.Element = ({
    contentText,
    siteUser,
    actions,
    user,
    forms,
    csrfToken,
    routes,
    etag,
}) => {
    const router = useRouter()
    const errorSummaryRef: any = useRef()

    const shouldRenderRoleForm: boolean = actions.includes(
        actionsConstants.SITE_ADMIN_MEMBERS_EDIT
    )

    const profileFormConfig: FormConfig = useFormConfig(
        formTypes.UPDATE_SITE_USER,
        forms[formTypes.UPDATE_SITE_USER]
    )

    const roleFormConfig: FormConfig = forms[formTypes.UPDATE_SITE_USER_ROLE]

    const [errors, setErrors] = useState(
        Object.assign(
            {},
            selectFormErrors(forms, formTypes.UPDATE_SITE_USER),
            selectFormErrors(forms, formTypes.UPDATE_SITE_USER_ROLE)
        )
    )

    const siteUserInitials: string = initials({
        value: `${siteUser.firstName} ${siteUser.lastName}`,
    })

    const { editHeading, editRoleHeading } = contentText ?? {}

    /**
     * Handle client-side validation failure in forms
     */
    const handleValidationFailure = (errors: FormErrors): void => {
        setErrors(errors)
        errorSummaryRef?.current?.focus?.()
    }

    /**
     * Handle client-side update submission for profile details
     */
    const handleProfileSubmit = async (
        formData: FormData
    ): Promise<FormErrors> => {
        return new Promise((resolve) => {
            const etagToUse: string =
                typeof etag === 'object' ? etag.profileEtag : etag

            const headers = getStandardServiceHeaders({
                csrfToken,
                etag: etagToUse,
            })

            putSiteUser({
                body: formData,
                user,
                headers,
                targetUserId: siteUser.id,
            })
                .then(() => {
                    setErrors({})
                    resolve({})

                    router.replace(`${routes.usersRoot}/${siteUser.id}`)
                })
                .catch((error) => {
                    const errors: FormErrors =
                        getServiceErrorDataValidationErrors(error) ||
                        getGenericFormError(error)

                    setErrors(errors)
                    resolve(errors)
                })
        })
    }

    /**
     * Handle client-side update submission for profile details
     */
    const handleRoleSubmit = async (
        formData: FormData
    ): Promise<FormErrors> => {
        return new Promise((resolve) => {
            const etagToUse: string =
                typeof etag === 'object' ? etag.roleEtag : etag

            const headers = getStandardServiceHeaders({
                csrfToken,
                etag: etagToUse,
            })

            putSiteUserRole({
                body: formData,
                user,
                headers,
                targetUserId: siteUser.id,
            })
                .then(() => {
                    setErrors({})
                    resolve({})

                    router.replace(`${routes.usersRoot}/${siteUser.id}`)
                })
                .catch((error) => {
                    const errors: FormErrors =
                        getServiceErrorDataValidationErrors(error) ||
                        getGenericFormError(error)

                    setErrors(errors)
                    resolve(errors)
                })
        })
    }

    /**
     * Render
     */
    return (
        <LayoutColumn className="c-page-body">
            <LayoutColumnContainer>
                <LayoutColumn tablet={6}>
                    <ErrorSummary
                        ref={errorSummaryRef}
                        errors={errors}
                        className="u-mb-10"
                    />
                    <h1>{editHeading}</h1>
                    <Avatar
                        image={siteUser.image}
                        initials={siteUserInitials}
                        className="u-h-36 u-w-36 u-mb-8"
                    />
                    <Form
                        csrfToken={csrfToken}
                        formConfig={profileFormConfig}
                        text={{
                            submitButton: 'Save changes',
                            cancelButton: 'Discard changes',
                        }}
                        submitAction={handleProfileSubmit}
                        cancelHref={`${routes.usersRoot}/${siteUser.id}`}
                        validationFailAction={handleValidationFailure}
                    />

                    {shouldRenderRoleForm && (
                        <>
                            <h2 className="u-mt-20">{editRoleHeading}</h2>
                            <Form
                                csrfToken={csrfToken}
                                formConfig={roleFormConfig}
                                validationFailAction={handleValidationFailure}
                                text={{
                                    submitButton: 'Update role',
                                }}
                                submitAction={handleRoleSubmit}
                            />
                        </>
                    )}
                </LayoutColumn>
            </LayoutColumnContainer>
        </LayoutColumn>
    )
}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (
    context: GetServerSidePropsContext
) =>
    await pipeSSRProps(
        context,
        {
            routeId: '9e86c5cc-6836-4319-8d9d-b96249d4c909',
        },
        [withUser, withRoutes, withTextContent],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get data from request context
             */
            const props: Partial<Props> = selectPageProps(context)
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
            const requestMethod: requestMethods = selectRequestMethod(context)

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

                    const updatedRolesForm: FormConfig = setFormConfigOptions(
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
                    if (submission && requestMethod === requestMethods.POST) {
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
        }
    )

/**
 * Export page template
 */
export default UserUpdatePage
