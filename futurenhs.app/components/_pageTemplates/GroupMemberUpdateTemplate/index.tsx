import { useRef, useState } from 'react'
import { useRouter } from 'next/router'

import { formTypes } from '@constants/forms'
import { actions as actionsConstants } from '@constants/actions'
import { selectForm } from '@selectors/forms'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { BackLink } from '@components/BackLink'
import { UserProfile } from '@components/UserProfile'
import { Form } from '@components/Form'
import { ErrorSummary } from '@components/ErrorSummary'
import { Dialog } from '@components/Dialog'
import { FormErrors, FormConfig } from '@appTypes/form'

import { Props } from './interfaces'
import { deleteGroupMember } from '@services/deleteGroupMember'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { getGenericFormError } from '@helpers/util/form'
import { putGroupMemberRole } from '@services/putGroupMemberRole'
import { useFormConfig } from '@hooks/useForm'

/**
 * Group member template
 */
export const GroupMemberUpdateTemplate: (props: Props) => JSX.Element = ({
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
    
    const deleteFormConfig: FormConfig = useFormConfig(formTypes.DELETE_GROUP_MEMBER, forms[formTypes.DELETE_GROUP_MEMBER])

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
            const headers = getStandardServiceHeaders({ csrfToken, etag })

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
            const headers = getStandardServiceHeaders({ csrfToken, etag })

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
            <BackLink
                href={routes.groupMembersRoot}
                text={{
                    link: 'Back',
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
