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
    isRequired?: boolean;
    shouldRenderRemainingCharacterCount?: boolean;
    className?: string;
}