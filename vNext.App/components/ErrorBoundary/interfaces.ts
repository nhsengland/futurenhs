export interface Props {
    text?: {
        error?: string;
    };
    children?: any;
    className?: string;
}

export interface State {
    hasError: boolean;
}