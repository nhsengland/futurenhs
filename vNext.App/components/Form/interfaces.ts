import { Field } from '@appTypes/form';

export interface Props {
    csrfToken: string;
    initialValues?: any;
    fields: Array<Field>;
    content: {
        submitButtonText: string;
    };
    action?: string;
    method?: string;
    submitAction: any;
    changeAction?: any;
    className?: string;
}