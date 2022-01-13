export interface Props {
    pathElementList: Array<{
        element: string;
        text: string;
    }>;
    content: {
        ariaLabelText: string;
        truncationText?: string;
    };
    seperatorIconName?: string;
    truncationMinPathLength?: number;
    truncationStartIndex?: number;
    truncationEndIndex?: number;
    className?: string;
}