import { FormField } from '@appTypes/form';

export interface Props {
    csrfToken: string;
    initialValues?: any;
    fields: Array<FormField>;
    text: {
        submitButton: string;
    };
    action?: string;
    method?: string;
    submitAction: any;
    changeAction?: any;
    className?: string;
    bodyClassName?: string;
    submitButtonClassName?: string;
}