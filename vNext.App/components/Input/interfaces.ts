export interface Props {
    input: {
        name: string;
        value: string | number;
        onChange: any;
    };
    meta: {
        error: string;
        submitError: string;
        touched: boolean;
    };
    content: {
        labelText: string;
        errorText?: string;
    };
    inputType?: 'text' | 'email' | 'password' | 'number';
    className?: string;
}