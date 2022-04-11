import { BreadCrumbList } from '@appTypes/routing'

export interface Props {
    breadCrumbList: BreadCrumbList
    text: {
        ariaLabel: string
        truncation?: string
    }
    seperatorIconName?: string
    truncationMinPathLength?: number
    truncationStartIndex?: number
    truncationEndIndex?: number
    shouldLinkCrumbs?: boolean
    className?: string
}
