import React from 'react'
import { render, screen, cleanup } from '@testing-library/react'

import { PaginationStatus } from './index'
import { Props } from './interfaces'

describe('Pagination Status', () => {
    const props: Props = {
        shouldEnableLoadMore: true,
        pageNumber: 1,
        pageSize: 10,
        totalRecords: 50,
    }

    it('renders correctly', () => {
        render(<PaginationStatus {...props} />)

        expect(screen.getAllByText('50').length).toBe(1)
    })

    it('does not render if page number, page size or total records are falsy', () => {
        const propsCopy: Props = Object.assign({}, props, { totalRecords: 0 })

        render(<PaginationStatus {...propsCopy} />)

        expect(screen.queryByText(/Showing/)).toBeNull()
    })

    it('renders passed prefix/suffix/infix values or defaults if not provided', () => {
        render(<PaginationStatus {...props} />)
        expect(screen.getAllByText(/Showing/).length).toBe(1)
        expect(screen.getAllByText(/of/).length).toBe(1)
        expect(screen.getAllByText(/items/).length).toBe(1)

        cleanup()

        const propsCopy: Props = Object.assign({}, props, {
            text: {
                prefix: 'prefix',
                infix: 'infix',
                suffix: 'suffix',
            },
        })

        render(<PaginationStatus {...propsCopy} />)

        expect(screen.getAllByText(/prefix/).length).toBe(1)
        expect(screen.getAllByText(/infix/).length).toBe(1)
        expect(screen.getAllByText(/suffix/).length).toBe(1)
    })

    it('renders correct `start` number on page 2 onwards', () => {
        const propsCopy: Props = Object.assign({}, props, {
            pageNumber: 2,
            shouldEnableLoadMore: false,
        })

        render(<PaginationStatus {...propsCopy} />)

        expect(screen.getAllByText('Showing 11 - 20').length).toBe(1)
    })
})
