export interface Props {
    id: string;
    isDisabled?: boolean;
    isOpen?: boolean;
    shouldCloseOnLeave?: boolean;
    shouldCloseOnContentClick?: boolean;
    shouldCloseOnRouteChange?: boolean;
    toggleAction?: (id: string, isOpen: boolean) => any;
    children: any;
    toggleOpenChildren: any;
    toggleClosedChildren: any;
    className?: string;
    toggleClassName?: string;
    contentClassName?: string;
}

