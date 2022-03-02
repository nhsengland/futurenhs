export interface Props {
    id: string;
    isDisabled?: boolean;
    isOpen?: boolean;
    shouldCloseOnLeave?: boolean;
    shouldCloseOnContentClick?: boolean;
    toggleAction?: (id: string, isOpen: boolean) => any;
    children: any;
    toggleChildren: any;
    className?: string;
    toggleClassName?: string;
    contentClassName?: string;
}

