import { Search } from './index'

export default {
    title: 'Search',
    component: Search,
    argTypes: {

    },
}

const Template = (args) => 
<div className="u-bg-theme-8 u-p-10">
    <Search {...args} />
</div>

export const Basic = Template.bind({})
Basic.args = {
    text: {
        label: 'Search the site',
        placeholder: 'Search for...'
    },
    className: ''
}

