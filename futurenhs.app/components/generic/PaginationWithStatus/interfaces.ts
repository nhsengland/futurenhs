import { Pagination } from '@appTypes/pagination'

export interface Props extends Pagination {
    id: string
    text?: {
        loadMore?: string
        previous?: string
        next?: string
    }
    visiblePages?: number
    shouldEnableLoadMore?: boolean
    shouldDisable?: boolean
    getPageAction?: (config: { pageNumber: number; pageSize: number }) => any
    className?: string
}
