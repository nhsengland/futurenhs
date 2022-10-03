import { TextArea } from './index'

export default {
    title: 'TextArea',
    component: TextArea,
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
    <TextArea input={{}} meta={{}} {...args} className="u-w-2/3" />
)

export const Basic = Template.bind({})
Basic.args = {
    text: {
        label: 'Text area label',
    },
}

export const RichTextEditor = Template.bind({})
RichTextEditor.args = {
    text: {
        label: 'Rich text editor label',
    },
    shouldRenderAsRte: true,
}
