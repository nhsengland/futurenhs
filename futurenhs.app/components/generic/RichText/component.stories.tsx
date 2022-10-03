import { RichText } from './index'

export default {
    title: 'RichText',
    component: RichText,
    argTypes: {

    },
}

const Template = (args) => <RichText {...args} />

export const Basic = Template.bind({})
Basic.args = {
    bodyHtml: '<p><b>Rich</b> text <i>content</i></p>'
}