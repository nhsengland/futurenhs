export interface Props {
    ref?: any;
    content: {
        bodyHtml: string;
    };
    errors?: Record<string, string>;
    relatedNames?: Array<string>;
    className?: string;
}

