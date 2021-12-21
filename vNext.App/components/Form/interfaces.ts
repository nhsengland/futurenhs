import { FormField } from '@appTypes/form';

export interface Props {
    csrfToken: string;
    initialValues?: any;
    fields: Array<FormField>;
    content: {
        submitButtonText: string;
    };
    action?: string;
    method?: string;
    submitAction: any;
    changeAction?: any;
    className?: string;
    bodyClassName?: string;
    submitButtonClassName?: string;
}