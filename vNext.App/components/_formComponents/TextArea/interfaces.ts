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
        hintHtml?: string;
        errorText?: string;
    };
    isRequired?: boolean;
    shouldRenderRemainingCharacterCount?: boolean;
    className?: string;
}