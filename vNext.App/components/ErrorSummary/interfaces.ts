export interface Props {
    ref?: any;
    text?: {
        body: string;
    };
    errors?: Record<string, string>;
    relatedNames?: Array<string>;
    className?: string;
}

