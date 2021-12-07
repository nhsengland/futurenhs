export interface Props {
    content: {
        titleText: string;
        navMenuAriaLabelText: string;
        copyrightText?: string;
    };
    navMenuList: Array<{
        url: string;
        text: string;
        isActive?: boolean;
    }>;
    className?: string;
}