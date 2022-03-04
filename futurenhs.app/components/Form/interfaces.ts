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
    submitAction: (formData: FormData) => Promise<any>;
    changeAction?: (props: any) => any;
    validationFailAction?: any;
    cancelHref?: string;
    className?: string;
    bodyClassName?: string;
    submitButtonClassName?: string;
    cancelButtonClassName?: string;
}