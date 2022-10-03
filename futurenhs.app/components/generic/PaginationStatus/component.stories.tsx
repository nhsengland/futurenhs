import React from 'react'
import { PaginationStatus } from './index'

export default {
    title: 'PaginationStatus',
    component: PaginationStatus,
    argTypes: {},
}

const Template = (args) => <PaginationStatus {...args} />

export const Basic = Template.bind({})
Basic.args = {
    totalRecords: 100,
    pageNumber: 1,
    pageSize: 10,
    text: {
        prefix: 'Showing',
        infix: 'of',
        suffix: 'items'
    },
    className: ''
}

export const LoadMore = Template.bind({})
LoadMore.args = {
    totalRecords: 100,
    pageNumber: 6,
    pageSize: 10,
    text: {
        prefix: 'Showing',
        infix: 'of',
        suffix: 'items'
    },
    shouldEnableLoadMore: true,
    className: ''
}

