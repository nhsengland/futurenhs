import { ActionLink } from './index'

export default {
    title: 'ActionLink',
    component: ActionLink,
}

const Template = (args) => <ActionLink {...args} />

export const Basic = Template.bind({})
Basic.args = {
    href: '/',
    text: {
        body: 'Edit',
        ariaLabel: 'Edit user',
    },
    iconName: 'icon-edit',
}
