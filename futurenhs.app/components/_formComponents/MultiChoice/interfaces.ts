export interface Props {
    input: {
        name: string;
        value: string | number;
        onChange: any;
        onBlur: any;
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
    options: Array<{
        name: string;
        label: string;
    }>
    inputType?: 'radio' | 'checkBox';
    isRequired?: boolean;
    isDisabled?: boolean;
    className?: string;
    optionClassName?: string;
}
