declare type Component = any;
declare type InputType = string;
declare type ValidatorType = string;

export interface FormField {
    component: Component;
    inputType?: InputType;
    name: string;
    text: {
        label?: string;
        legend?: string;
        hint?: string;
    };
    fields?: Array<FormField>;
    options?: Array<{
        value: string;
        label: string;
    }>;
    shouldRenderAsRte?: boolean;
    shouldRenderRemainingCharacterCount?: boolean;
    validators?: Array<{
        type: ValidatorType;
        maxLength?: number;
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