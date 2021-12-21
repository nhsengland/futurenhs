declare type Component = any;
declare type InputType = string;
declare type ValidatorType = string;

export interface FormField {
    component: Component;
    inputType?: InputType;
    name: string;
    content: {
        labelText?: string;
        hintHtml?: string;
    };
    validators?: Array<{
        type: ValidatorType;
        message: string;
    }>;
    className?: string;
}

export interface FormStep {
    fields: Array<FormField>;
}

export interface Form {
    steps: Array<FormStep>;
}