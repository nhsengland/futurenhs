import { Pagination } from './index'

export default {
    title: 'Pagination',
    component: Pagination,
    argTypes: {

    },
}

const Template = (args) => 
<div className="">
    <Pagination {...args} />
</div>

export const Basic = Template.bind({})
Basic.args = {
    id: 'pagination',
    totalRecords: 100,
    pageNumber: 1,
    pageSize: 10,
    text: {
        loadMore: 'Load more',
        previous: 'Previous',
        next: 'Next'
    },
    className: ''
}

export const LoadMore = Template.bind({})
LoadMore.args = {
    id: 'pagination',
    totalRecords: 100,
    pageNumber: 1,
    pageSize: 10,
    text: {
        loadMore: 'Load more',
        previous: 'Previous',
        next: 'Next'
    },
    shouldEnableLoadMore: true,
    className: ''
}

