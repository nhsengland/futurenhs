export interface Props {
    input: {
        name: string;
        value: string;
        onChange: any;
    };
    meta: {
        error: string;
        submitError: string;
        touched: boolean;
    };
    initialError?: string;
    text: {
        label: string;
        hint?: string;
        error?: string;
    };
    inputType?: 'text' | 'email' | 'password' | 'number' | 'file';
    shouldRenderRemainingCharacterCount?: boolean;
    relatedFields?: Record<string, string>;
    validators?: Array<any>;
    className?: string;
}