import { ActionLink } from './index'

export default {
    title: 'ActionLink',
    component: ActionLink,
    argTypes: {
        iconName: {
            options: (window.parent as any).sbIconList,
            control: { type: 'select' },
        },
    }
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
