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
    text: {
        label: string;
        hint?: string;
        error?: string;
    };
    inputType?: 'text' | 'email' | 'password' | 'number';
    isRequired?: boolean;
    shouldRenderRemainingCharacterCount?: boolean;
    className?: string;
}