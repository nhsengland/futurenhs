export interface Props {
    content: {
        ariaLabelText: string;
    }
    navMenuList?: Array<{
        url: string;
        text: string;
    }>;
}