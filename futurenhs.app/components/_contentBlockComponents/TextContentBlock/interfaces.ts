export interface Props {
    id?: string
    text: {
        heading: string;
        bodyHtml: string;
    };
    headingLevel: number;
    className?: string
}
