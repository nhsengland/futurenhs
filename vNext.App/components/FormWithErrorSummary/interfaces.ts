import { Field } from '@appTypes/form';

export interface Props {
    csrfToken: string;
    initialValues?: any;
    fields: Array<Field>;
    errors: Record<string, string>;
    content: {
        errorSummary: {
            bodyHtml: string;
        },
        form: {
            submitButtonText: string;
        }
    }
    action?: string;
    method?: string;
    submitAction: any;
    changeAction?: any;
    className?: string;
    children?: any;
}