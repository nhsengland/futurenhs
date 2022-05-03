export interface Props {
    id?: string
    text: {
        heading: string;
    };
    headingLevel: number;
    links: Array<{
        url: string
        text: string
    }>;
    themeId?: string;
    className?: string
}
