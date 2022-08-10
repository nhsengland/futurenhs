export interface Props {
    isActive: boolean;
    text: {
        loadingMessage: string
    };
    delay: number;
    remain?: number;
    className?: string;
}
