import { BreadCrumbList } from '@appTypes/routing'

export interface Props {
    /**
     * List of links to be displayed
     */
    breadCrumbList: BreadCrumbList
    /**
     * Controls hidden aria label and optional truncation text override, renders ellipsis by default
     */
    text: {
        ariaLabel: string
        truncation?: string
    }
    /**
     * Overrides default seperator icon
     */
    seperatorIconName?: string
    /**
     * Sets minimum length of breadcrumb list before truncation is applied
     */
    truncationMinPathLength?: number
    /**
     * Sets index of the item in the list where truncation should begin
     */
    truncationStartIndex?: number
    /**
     * Sets index of the item in the list where truncation should end
     */
    truncationEndIndex?: number
    /**
     * Determines whether links for each crumb should be concatenated e.g /crumb1 > /crumb1/crumb2 > /crumb1/crumb2/crumb3
     */
    shouldLinkCrumbs?: boolean
    className?: string
}
