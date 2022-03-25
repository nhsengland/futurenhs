declare type Component = any;
declare type InputType = string;
declare type ValidatorType = string;

export interface FormField {
    component: Component;
    inputType?: InputType;
    name?: string;
    text?: {
        label?: string;
        legend?: string;
        hint?: string;
    };
    fields?: Array<FormField>;
    options?: Array<{
        value: string;
        label: string;
    }>;
    initialError?: string;
    shouldRenderAsRte?: boolean;
    shouldRenderRemainingCharacterCount?: boolean;
    validators?: Array<{
        type: ValidatorType;
        maxLength?: number;
        maxFileSize?: number;
        validFileExtensions?: Array<string>;
        message: string;
    }>;
    className?: string;
    optionClassName?: string;
}

export interface FormStep {
    fields: Array<FormField>;
}

export interface FormConfig {
    id: string;
    initialValues?: Record<string, any>;
    errors?: Record<string, string>;
    steps: Array<FormStep>;
}

export type FormErrors = Record<string, string>;