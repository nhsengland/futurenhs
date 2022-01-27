import { FormField } from '@appTypes/form';

export interface Props {
    csrfToken: string;
    initialValues?: any;
    fields: Array<FormField>;
    errors: Array<Record<string, string>>;
    text: {
        errorSummary: {
            body: string;
        },
        form: {
            submitButton: string;
        }
    }
    action?: string;
    method?: string;
    submitAction: any;
    changeAction?: any;
    className?: string;
    bodyClassName?: string;
    submitButtonClassName?: string;
    children?: any;
}