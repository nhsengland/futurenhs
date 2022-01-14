import { BreadCrumbList } from '@appTypes/routing';

export interface Props {
    breadCrumbList: BreadCrumbList;
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