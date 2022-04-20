import { Service } from "@appTypes/service";
import { services as serviceId } from "@constants/services";

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
    shouldPreventFreeText?: boolean;
    minimumCharacters?: number;
    validators?: Array<any>;
    context?: Record<string, any>;
    services?: Record<string, Service>;
    serviceId?: serviceId;
    className?: string;
}