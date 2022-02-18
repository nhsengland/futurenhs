export interface Props {
    value?: any;
    method: 'GET' | 'POST' | 'PUT' | 'PATCH';
    action: string;
    id: string;
    text: {
        label: string;
        placeholder: string;
    };
    className?: string;
}

