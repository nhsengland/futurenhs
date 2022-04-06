import { FormField } from '@appTypes/form';

export interface Props {
    csrfToken: string;
    formId: string;
    instanceId?: string;
    initialValues?: any;
    fields: Array<FormField>;
    errors: Record<string, string>;
    text: {
        errorSummary: {
            body: string;
        },
        form: {
            submitButton: string;
            cancelButton?: string;
        }
    }
    action?: string;
    method?: string;
    cancelHref?: string;
    cancelAction?: () => any;
    submitAction: (formData: FormData) => Promise<Record<string, string>>;
    changeAction?: any;
    className?: string;
    bodyClassName?: string;
    submitButtonClassName?: string;
    shouldClearOnSubmitSuccess?: boolean;
    children?: any;
}