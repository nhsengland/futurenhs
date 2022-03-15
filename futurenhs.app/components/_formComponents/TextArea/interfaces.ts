export interface Props {
    input: {
        name: string;
        value: any;
        onChange: any;
        onBlur;
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
    shouldRenderAsRte?: boolean;
    shouldRenderRemainingCharacterCount?: boolean;
    validators?: Array<any>;
    className?: string;
}