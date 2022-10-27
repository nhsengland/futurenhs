import { GetServerSideProps } from 'next'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { routeParams } from '@constants/routes'
import { formTypes } from '@constants/forms'
import { layoutIds, groupTabIds } from '@constants/routes'
import { withUser } from '@helpers/hofs/withUser'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { withGroup } from '@helpers/hofs/withGroup'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { getGroupMember } from '@services/getGroupMember'
import { getGroupRoles } from '@services/getGroupRoles'
import {
    selectUser,
    selectParam,
    selectCsrfToken,
    selectFormData,
    selectRequestMethod,
    selectPageProps,
} from '@helpers/selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { actions } from '@constants/actions'
import { checkMatchingFormType, setFormConfigOptions } from '@helpers/util/form'
import { requestMethods } from '@constants/fetch'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { putGroupMemberRole } from '@services/putGroupMemberRole'
import { deleteGroupMember } from '@services/deleteGroupMember'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { selectForm } from '@helpers/selectors/forms'
import formConfigs from '@config/form-configs/index'
import { useRef, useState } from 'react'
import { useRouter } from 'next/router'
import { actions as actionsConstants } from '@constants/actions'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { UserProfile } from '@components/generic/UserProfile'
import { Form } from '@components/forms/Form'
import { ErrorSummary } from '@components/generic/ErrorSummary'
import { Dialog } from '@components/generic/Dialog'
import { FormErrors, FormConfig, FormOptions } from '@appTypes/form'
import { getGenericFormError } from '@helpers/util/form'
import { useFormConfig } from '@helpers/hooks/useForm'
import { Image } from '@appTypes/image'
import { ActionLink } from '@components/generic/ActionLink'
import { GroupPage } from '@appTypes/page'
import { GroupMember } from '@appTypes/group'

export interface Props extends GroupPage {
    member: GroupMember
}

/**
 * Group member template
 */
export const GroupMemberUpdatePage: (props: Props) => JSX.Element = ({
    csrfToken,
    contentText,
    member,
    forms,
    actions,
    routes,
    user,
    groupId,
    etag,
}) => {
    const router = useRouter()
    const errorSummaryRef: any = useRef()
    const [
        isDeleteUserConfirmationModalOpen,
        setIsDeleteUserConfirmationModalOpen,
    ] = useState(false)

    const updateFormConfig: FormConfig = forms[formTypes.UPDATE_GROUP_MEMBER]

    const deleteFormConfig: FormConfig = useFormConfig(
        formTypes.DELETE_GROUP_MEMBER,
        forms[formTypes.DELETE_GROUP_MEMBER]
    )

    const [errors, setErrors] = useState(
        updateFormConfig?.errors || deleteFormConfig?.errors
    )

    const {
        secondaryHeading,
        firstNameLabel,
        lastNameLabel,
        pronounsLabel,
        emailLabel,
    } = contentText ?? {}

    const shouldRenderDeleteForm: boolean = actions.includes(
        actionsConstants.GROUPS_MEMBERS_DELETE
    )

    const memberProfileImage: Image = member.image

    /**
     * Handle client-side validation failure in forms
     */
    const handleValidationFailure = (errors: FormErrors): void => {
        setErrors(errors)
        errorSummaryRef?.current?.focus?.()
    }

    /**
     * Handle client-side update submission
     */
    const handleUpdateMemberSubmit = async (
        formData: FormData
    ): Promise<FormErrors> => {
        return new Promise((resolve) => {
            const headers = getStandardServiceHeaders({
                csrfToken,
                etag,
                accessToken: user.accessToken,
            })

            putGroupMemberRole({
                user,
                headers,
                body: formData,
                groupId,
                memberId: member.id,
            })
                .then(() => {
                    router.replace(`${routes.groupMembersRoot}`)
                    resolve({})
                })
                .catch((error) => {
                    const errors: FormErrors =
                        getServiceErrorDataValidationErrors(error) ||
                        getGenericFormError(error)

                    setErrors(errors)
                    resolve(errors)
                })

            resolve({})
        })
    }

    /**
     * Handle client-side delete submission
     */
    const handleDeleteMemberSubmit = async (): Promise<FormErrors> => {
        return new Promise((resolve) => {
            setIsDeleteUserConfirmationModalOpen(true)

            resolve({})
        })
    }

    /**
     * Handle client-side delete submission cancellation
     */
    const handleDeleteMemberSubmitCancel = (): void =>
        setIsDeleteUserConfirmationModalOpen(false)

    /**
     * Handle client-side delete submission confirmation
     */
    const handleDeleteMemberSubmitConfirm = async (): Promise<FormErrors> => {
        return new Promise((resolve) => {
            const headers = getStandardServiceHeaders({
                csrfToken,
                etag,
                accessToken: user.accessToken,
            })

            deleteGroupMember({
                groupId,
                groupUserId: member.id,
                user,
                headers,
            })
                .then(() => {
                    setIsDeleteUserConfirmationModalOpen(false)
                    router.replace(`${routes.groupMembersRoot}`)
                    resolve({})
                })
                .catch((error) => {
                    const errors: FormErrors =
                        getServiceErrorDataValidationErrors(error) ||
                        getGenericFormError(error)

                    setErrors(errors)
                    setIsDeleteUserConfirmationModalOpen(false)
                    resolve(errors)
                })

            resolve({})
        })
    }

    /**
     * Render
     */
    return (
        <LayoutColumn className="c-page-body">
            <ActionLink
                href={routes.groupMembersRoot}
                iconName="icon-chevron-left"
                className="u-mb-8"
                text={{
                    body: 'Back',
                    ariaLabel: 'Go back to list of group members',
                }}
            />
            <ErrorSummary
                ref={errorSummaryRef}
                errors={errors}
                className="u-mb-10"
            />
            <LayoutColumnContainer justify="centre">
                <LayoutColumn tablet={11}>
                    <UserProfile
                        image={memberProfileImage}
                        profile={member}
                        text={{
                            heading: secondaryHeading,
                            firstNameLabel: firstNameLabel,
                            lastNameLabel: lastNameLabel,
                            pronounsLabel: pronounsLabel,
                            emailLabel: emailLabel,
                        }}
                        className="tablet:u-justify-center tablet:u-mt-16"
                    >
                        <Form
                            csrfToken={csrfToken}
                            formConfig={updateFormConfig}
                            text={{
                                submitButton: 'Save Changes',
                            }}
                            submitAction={handleUpdateMemberSubmit}
                            validationFailAction={handleValidationFailure}
                            className="u-mt-14"
                        />
                        {shouldRenderDeleteForm && (
                            <Form
                                csrfToken={csrfToken}
                                formConfig={deleteFormConfig}
                                text={{
                                    submitButton: 'Remove from group',
                                }}
                                submitAction={handleDeleteMemberSubmit}
                                className="u-mt-14"
                                submitButtonClassName="c-button-outline"
                            />
                        )}
                    </UserProfile>
                </LayoutColumn>
            </LayoutColumnContainer>
            <Dialog
                id="dialog-delete-group-member"
                isOpen={isDeleteUserConfirmationModalOpen}
                text={{
                    cancelButton: 'Cancel',
                    confirmButton: 'Yes, remove',
                    heading: 'Remove member',
                }}
                cancelAction={handleDeleteMemberSubmitCancel}
                confirmAction={handleDeleteMemberSubmitConfirm}
            >
                <p className="u-text-bold">
                    This member will be removed from the group. Are you sure you
                    wish to proceed?
                </p>
            </Dialog>
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
            routeId: '4502d395-7c37-4e80-92b7-65886de858ef',
        },
        [withUser, withRoutes, withGroup, withTextContent],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get data from request context
             */
            const props: Partial<any> = selectPageProps(context)
            const user: User = selectUser(context)
            const csrfToken: string = selectCsrfToken(context)
            const currentValues: any = selectFormData(context)
            const requestMethod: requestMethods = selectRequestMethod(context)

            if (!props.actions.includes(actions.GROUPS_MEMBERS_EDIT)) {
                return {
                    notFound: true,
                }
            }

            const groupId: string = selectParam(context, routeParams.GROUPID)
            const memberId: string = selectParam(context, routeParams.MEMBERID)

            props.forms = {
                [formTypes.UPDATE_GROUP_MEMBER]: selectForm(
                    formConfigs,
                    formTypes.UPDATE_GROUP_MEMBER
                ),
                [formTypes.DELETE_GROUP_MEMBER]: {},
            }

            const form: any = props.forms[formTypes.UPDATE_GROUP_MEMBER]

            const deleteMemberForm: any =
                props.forms[formTypes.DELETE_GROUP_MEMBER]

            /**
             * Get data from services
             */
            try {
                const [memberData, groupRoles] = await Promise.all([
                    getGroupMember({
                        groupId,
                        user,
                        memberId,
                        isForUpdate: true,
                    }),
                    getGroupRoles({ groupId, user }),
                ])
                const etag = memberData.headers?.get('etag')
                props.etag = etag

                props.member = memberData.data
                props.layoutId = layoutIds.GROUP
                props.tabId = groupTabIds.MEMBERS
                props.pageTitle = `${props.entityText.title} - ${
                    props.member.firstName ?? ''
                } ${props.member.lastName ?? ''} - Edit`

                /**
                 * Handle setting role options for multi-choice
                 */
                const roleOptions: Array<FormOptions> = groupRoles?.data?.map(
                    (role) => {
                        return {
                            value: role.id,
                            label: role.name,
                        }
                    }
                )

                const updatedRolesForm = setFormConfigOptions(
                    form,
                    0,
                    'groupUserRoleId',
                    roleOptions
                )
                const usersCurrentRole = groupRoles.data?.find(
                    (role) => role.name === props.member?.role
                )

                props.forms[formTypes.UPDATE_GROUP_MEMBER] = updatedRolesForm

                updatedRolesForm.initialValues = {
                    groupUserRoleId: usersCurrentRole?.id,
                }

                /**
                 * Handle server-side form post
                 */
                if (currentValues && requestMethod === requestMethods.POST) {
                    const headers = getStandardServiceHeaders({
                        csrfToken,
                        etag,
                        accessToken: user.accessToken,
                    })

                    const isRoleForm = checkMatchingFormType(
                        currentValues,
                        updatedRolesForm.id
                    )
                    const isDeleteForm = checkMatchingFormType(
                        currentValues,
                        formTypes.DELETE_GROUP_MEMBER
                    )

                    if (isRoleForm) {
                        updatedRolesForm.initialValues = currentValues.getAll()

                        await putGroupMemberRole({
                            headers,
                            user,
                            body: currentValues,
                            groupId,
                            memberId,
                        })
                    }

                    if (isDeleteForm) {
                        await deleteGroupMember({
                            groupId,
                            groupUserId: memberId,
                            headers,
                            user,
                        })
                    }

                    return {
                        redirect: {
                            permanent: false,
                            destination: `${props.routes.groupMembersRoot}`,
                        },
                    }
                }
            } catch (error) {
                const validationErrors: FormErrors =
                    getServiceErrorDataValidationErrors(error)

                if (validationErrors) {
                    deleteMemberForm.errors = validationErrors
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
export default GroupMemberUpdatePage
