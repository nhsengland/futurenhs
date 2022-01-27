export interface Props {
    ref?: any;
    text?: {
        body: string;
    };
    errors?: Array<Record<string, string>>;
    relatedNames?: Array<string>;
    className?: string;
}

