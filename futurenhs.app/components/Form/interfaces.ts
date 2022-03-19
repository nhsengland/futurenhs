import { FormField } from '@appTypes/form';

export interface Props {
    csrfToken: string;
    initialValues?: Record<string, any>;
    initialErrors?: Record<string, string>;
    formId: string;
    instanceId?: string;
    fields: Array<FormField>;
    text: {
        submitButton: string;
        cancelButton?: string;
    };
    action?: string;
    method?: string;
    submitAction: (formData: FormData) => Promise<Record<string, string>>;
    changeAction?: (props: any) => any;
    cancelAction?: () => any;
    validationFailAction?: any;
    cancelHref?: string;
    className?: string;
    bodyClassName?: string;
    submitButtonClassName?: string;
    cancelButtonClassName?: string;
    shouldAddErrorTitle?: boolean;
    shouldClearOnSubmitSuccess?: boolean;
}