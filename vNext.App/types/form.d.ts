declare type Component = any;
declare type InputType = string;
declare type ValidatorType = string;

export interface Field {
    component: Component;
    inputType?: InputType;
    name: string;
    content: {
        labelText: string;
    };
    validators?: Array<{
        type: ValidatorType;
        message: string;
    }>;
}