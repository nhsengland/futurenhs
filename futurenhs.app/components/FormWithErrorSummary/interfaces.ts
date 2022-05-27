import { FormConfig, FormErrors } from '@appTypes/form'

export interface Props {
    csrfToken: string
    instanceId?: string
    formConfig: FormConfig
    errors: FormErrors
    context?: Record<string, any>
    text: {
        errorSummary?: {
            body: string
        }
        form: {
            submitButton: string
            cancelButton?: string
        }
    }
    action?: string
    method?: string
    cancelHref?: string
    cancelAction?: () => any
    submitAction: (formData: FormData) => Promise<Record<string, string>>
    changeAction?: any
    className?: string
    bodyClassName?: string
    submitButtonClassName?: string
    shouldClearOnSubmitSuccess?: boolean
    children?: any
}
