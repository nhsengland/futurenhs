export interface Props {
    content?: {
        errorText?: string;
    };
    children?: any;
    className?: string;
}

export interface State {
    hasError: boolean;
}