import { requestMethods } from '@constants/fetch';

export interface Props {
    value?: any;
    method: requestMethods;
    action: string;
    id: string;
    text: {
        label: string;
        placeholder: string;
    };
    className?: string;
}

