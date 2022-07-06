import { useState, useEffect } from 'react'
import { Props } from './interfaces'

/**
 * Derived from the Home Office pagination component: https://design.homeoffice.gov.uk/components?name=Pagination.
 * Supports progressive enhancement to work with a load more button.
 */
export const PaginationStatus: (props: Props) => JSX.Element = ({
    text = {
        prefix: 'Showing',
        infix: 'of',
        suffix: 'items',
    },
    shouldEnableLoadMore,
    pageNumber,
    pageSize,
    totalRecords,
}) => {
    if (!pageNumber || !pageSize || !totalRecords || totalRecords < pageSize) {
        return null
    }

    const [isLoadMoreEnabled, setIsLoadMoreEnabled] = useState(false)

    const { prefix, infix, suffix } = text

    const currentEnd: number = pageNumber * pageSize
    const start: number =
        pageNumber === 1 || isLoadMoreEnabled
            ? 1
            : (pageNumber - 1) * pageSize + 1
    const end: number = currentEnd < totalRecords ? currentEnd : totalRecords

    useEffect(() => {
        setIsLoadMoreEnabled(shouldEnableLoadMore)
    }, [shouldEnableLoadMore])

    return (
        <p className="c-pagination-status">
            <span className="u-text-bold">{`${prefix} ${start} - ${end}`}</span>{' '}
            {`${infix}`} <span className="u-text-bold">{totalRecords}</span>{' '}
            {`${suffix}`}
        </p>
    )
}
