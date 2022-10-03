import { Spinner } from './index'

export default {
    title: 'Spinner',
    component: Spinner,
    argTypes: {
        speed: {
            control: { options: ['slow', 'medium', 'fast'] },
        },
    },
}

const Template = (args) => 
<div className="u-w-[180px] u-h-[180px]">
    <Spinner {...args} />
</div>

export const Basic = Template.bind({})
Basic.args = {
    speed: 'medium',
    className: ''
}

