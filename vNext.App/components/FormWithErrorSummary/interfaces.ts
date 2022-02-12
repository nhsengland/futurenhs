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
    submitAction: any;
    changeAction?: any;
    className?: string;
    bodyClassName?: string;
    submitButtonClassName?: string;
    children?: any;
}