import { ClickLink } from './index'

export default {
    title: 'ClickLink',
    component: ClickLink,
    argTypes: {
        iconName: {
            options: (window.parent as any).sbIconList,
            control: { type: 'select' },
        },
    },
}

const Template = (args) => <ClickLink {...args} />

export const Basic = Template.bind({})
Basic.args = {
    onClick: () => null,
    text: {
        body: 'Edit',
        ariaLabel: 'Edit user',
    },
    iconName: 'icon-edit',
}
