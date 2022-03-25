export interface Props {
    id: string;
    isOpen?: boolean;
    children?: any;
    appElement?: HTMLElement;
    text: {
        confirmButton: string;
        cancelButton?: string;
    };
    confirmAction?: any;
    cancelAction?: any;
    className?: string;
}

