import { BackLink } from './index'

export default {
    title: 'BackLink',
    component: BackLink,
}

const Template = (args) => <BackLink {...args} />

export const Basic = Template.bind({})
Basic.args = {
    href: '/',
    text: {
        link: 'Back',
    },
}
