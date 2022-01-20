export interface Props {
    text: {
        ariaLabel: string;
    }
    navMenuList?: Array<{
        url: string;
        text: string;
        isActive?: boolean;
    }>;
}