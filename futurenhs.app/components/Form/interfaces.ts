import { FormField } from '@appTypes/form';

export interface Props {
    csrfToken: string;
    initialValues?: any;
    formId: string;
    instanceId?: string;
    fields: Array<FormField>;
    text: {
        submitButton: string;
        cancelButton?: string;
    };
    action?: string;
    method?: string;
    submitAction: any;
    validationFailAction?: any;
    changeAction?: any;
    cancelHref?: string;
    className?: string;
    bodyClassName?: string;
    submitButtonClassName?: string;
    cancelButtonClassName?: string;
}