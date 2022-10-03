import { Loader } from './index'

export default {
    title: 'Loader',
    component: Loader,
    argTypes: {
    },
}

const Template = (args) => <div className="u-relative u-h-[400px]"><Loader {...args} /></div>

export const Basic = Template.bind({})
Basic.args = {
    isActive: true,
    delay: 1000,
    text: {
        loadingMessage: 'Loading, please wait'
    },
    className: ''
}

