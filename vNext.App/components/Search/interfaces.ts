export interface Props {
    method: 'GET' | 'POST' | 'PUT' | 'PATCH';
    action: string;
    id: string;
    content: {
        labelText: string;
        placeholderText: string;
    };
    className?: string;
}

