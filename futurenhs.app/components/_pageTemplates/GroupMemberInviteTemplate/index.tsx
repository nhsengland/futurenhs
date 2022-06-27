import { FormConfig, FormErrors } from '@appTypes/form'
import { FormWithErrorSummary } from '@components/FormWithErrorSummary'
import { LayoutColumn } from '@components/LayoutColumn'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { formTypes } from '@constants/forms'
import { notifications } from '@constants/notifications'
import { Notification } from '@components/NotificationBanner/interfaces'
import { NotificationsContext } from '@contexts/index'
import { getGenericFormError } from '@helpers/util/form'
import { useFormConfig } from '@hooks/useForm'
import { useNotification } from '@hooks/useNotification'
import { getServiceErrorDataValidationErrors } from '@services/index'
import { postGroupMemberInvite } from '@services/postGroupMemberInvite'
import { useContext, useEffect, useState } from 'react'
import { Props } from './interfaces'

/**
 * Group member invite template
 */
export const GroupMemberInviteTemplate: (props: Props) => JSX.Element = ({
    csrfToken,
    forms,
    user,
    contentText,
    groupId,
}) => {
    const formConfig: FormConfig = useFormConfig(
        formTypes.INVITE_USER,
        forms[formTypes.INVITE_USER]
    )
    const [errors, setErrors] = useState(formConfig?.errors)
    const notificationsContext: any = useContext(NotificationsContext)

    // useNotification('Test notification', notifications.SUCCESS)
    
   

    const { secondaryHeading } = contentText

    /**
     * Client-side submission handler - TODO: Pending API
     */
    const handleSubmit = async (formData: FormData): Promise<FormErrors> => {
        // try {
        //     await postGroupMemberInvite({
        //         user,
        //         body: formData as any,
        //         groupId,
        //     })

        //     return Promise.resolve({})
        // } catch (error) {
        //     const errors: FormErrors =
        //         getServiceErrorDataValidationErrors(error) ||
        //         getGenericFormError(error)

        //     setErrors(errors)

        //     return Promise.resolve(errors)
        // }

        const emailAddress: FormDataEntryValue = formData.get('Email')
        useNotification(notificationsContext, `Invite sent to ${emailAddress}`, notifications.SUCCESS)
        
        return Promise.resolve(errors)
    }

    /**
     * Render
     */
    return (
        <LayoutColumn className="c-page-body">
            <LayoutColumnContainer>
                <LayoutColumn tablet={8}>
                    <FormWithErrorSummary
                        csrfToken={csrfToken}
                        formConfig={formConfig}
                        errors={errors}
                        text={{
                            form: {
                                submitButton: 'Send invite',
                            },
                        }}
                        submitAction={handleSubmit}
                        shouldClearOnSubmitSuccess={true}
                    >
                        <h2 className="nhsuk-heading-l">{secondaryHeading}</h2>
                    </FormWithErrorSummary>
                </LayoutColumn>
            </LayoutColumnContainer>
        </LayoutColumn>
    )
}
