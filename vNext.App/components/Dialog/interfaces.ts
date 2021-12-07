export interface Props {
    id: string;
    isOpen?: boolean;
    children?: any;
    appElement?: HTMLElement;
    content: {
        confirmButtonText: string;
        cancelButtonText?: string;
    };
    confirmAction?: any;
    cancelAction?: any;
    className?: string;
}

