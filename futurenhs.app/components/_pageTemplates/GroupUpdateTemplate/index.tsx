import { useEffect, useState } from 'react'
import { useRouter } from 'next/router'
import { useFormConfig } from '@hooks/useForm'
import { formTypes } from '@constants/forms'
import { getStandardServiceHeaders } from '@helpers/fetch'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { selectForm } from '@selectors/forms'
import { FormWithErrorSummary } from '@components/FormWithErrorSummary'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { putGroup } from '@services/putGroup'
import { FormErrors, FormConfig, FormField } from '@appTypes/form'

import { Props } from './interfaces'
import { getFormField, getGenericFormError } from '@helpers/util/form'
import { Dialog } from '@components/Dialog'

/**
 * Group create folder template
 */
export const GroupUpdateTemplate: (props: Props) => JSX.Element = ({
    groupId,
    user,
    csrfToken,
    etag,
    forms,
    routes,
    contentText,
    isPublic,
    services = {
        putGroup: putGroup,
    },
}) => {
    const router = useRouter()
    const [isChangeGroupPrivacyModalOpen, setIsChangeGroupPrivacyModalOpen] = useState(false)
    const [groupEditFormData, setGroupEditFormData] = useState({})

 
    const updateGroupFormValues: FormConfig = forms[formTypes.UPDATE_GROUP]

    const formConfig: FormConfig = useFormConfig(formTypes.UPDATE_GROUP, updateGroupFormValues.initialValues, updateGroupFormValues.errors)
    const [errors, setErrors] = useState(formConfig?.errors)

    const groupPrivacyField: FormField = getFormField(formConfig, 'isPublic');

    /**
    * Hides group is public checkbox if group is already private
    */
    if (!isPublic && groupPrivacyField) {

            groupPrivacyField.shouldRender = false

    }


    const { mainHeading, secondaryHeading } = contentText ?? {}

    /**
     * Handle client-side update submission
     */
    const handleSubmit = async (formData: FormData): Promise<FormErrors> => {

        /**
         * If checkbox is unticked, store the form data and reveal confirmation dialog. Submission is then handled by the dialog confirm.
         */
        setGroupEditFormData(formData)
        const isPublicBoxTicked: boolean = Boolean(formData.get('isPublic'))
        
        if (!isPublicBoxTicked && !isChangeGroupPrivacyModalOpen && groupPrivacyField.shouldRender !== false) {
            setIsChangeGroupPrivacyModalOpen(true)
            return
        }

        return new Promise((resolve) => {
            const headers = getStandardServiceHeaders({ csrfToken, etag })

            services
                .putGroup({ groupId, user, headers, body: formData })
                .then(() => {
                    setIsChangeGroupPrivacyModalOpen(false)
                    setErrors({})
                    resolve({})

                    router.replace(routes.groupRoot)

                    /**
                     * Full page reload currently necessary to clear image cache of previous group image
                     */
                    window.location.replace(routes.groupRoot)
                })
                .catch((error) => {
                    const errors: FormErrors =
                        getServiceErrorDataValidationErrors(error) ||
                        getGenericFormError(error)

                    setIsChangeGroupPrivacyModalOpen(false)
                    setErrors(errors)
                    resolve(errors)
                })
        })
    }

    const handleSubmitConfirm = (): void => {
        handleSubmit(groupEditFormData as any)
    }

    const handleSubmitCancel = (): void => {
        setIsChangeGroupPrivacyModalOpen(false)
    }


    /**
     * Render
     */
    return (
        <>
            <LayoutColumn className="c-page-body">
                <LayoutColumnContainer>
                    <LayoutColumn tablet={8}>
                        <FormWithErrorSummary
                            csrfToken={csrfToken}
                            formConfig={formConfig}
                            errors={errors}
                            text={{
                                form: {
                                    submitButton: 'Save and close',
                                    cancelButton: 'Discard changes',
                                },
                            }}
                            cancelHref={routes.groupRoot}
                            submitAction={handleSubmit}
                            bodyClassName="u-mb-12"
                        >
                            <h2 className="nhsuk-heading-l">{mainHeading}</h2>
                            <p className="u-text-lead u-text-theme-7 u-mb-4">
                                {secondaryHeading}
                            </p>
                        </FormWithErrorSummary>
                    </LayoutColumn>
                </LayoutColumnContainer>
                <Dialog
                    id="dialog-change-group-privacy"
                    isOpen={isChangeGroupPrivacyModalOpen}
                    text={{
                        cancelButton: 'Cancel',
                        confirmButton: 'Yes, submit',
                        heading: 'Change group privacy'
                    }}
                    cancelAction={handleSubmitCancel}
                    confirmAction={handleSubmitConfirm}
                >
                    <p className="u-text-bold">
                        Unselecting 'Group is public?' will set this group to private, restricting access to approved members only. This cannot be undone, are you sure you wish to continue?
                    </p>
                </Dialog>
            </LayoutColumn>
        </>
    )
}
