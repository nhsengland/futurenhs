import { CheckBox } from './index'

export default {
    title: 'CheckBox',
    component: CheckBox,
    argTypes: {
        input: {
            control: { type: '' },
        },
        meta: {
            control: { type: '' },
        },
        initialError: {
            control: { type: '' },
        },
        validators: {
            control: { type: '' },
        },
    },
}

const Template = (args) => (
    <CheckBox input={{}} meta={{}} {...args} className="u-w-2/3" />
)

export const Basic = Template.bind({})
Basic.args = {
    input: {
        name: 'checkbox-id',
        onChange: () => {},
        onBlur: () => {},
        onFocus: () => {},
    },
    text: {
        label: 'Example checkbox label',
    },
}
