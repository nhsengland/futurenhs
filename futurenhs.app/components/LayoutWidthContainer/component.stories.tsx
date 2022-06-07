import { LayoutWidthContainer } from './index'

export default {
    title: 'LayoutWidthContainer',
    component: LayoutWidthContainer
}

const Template = (args) => <LayoutWidthContainer {...args}><p className="u-m-4">A layout component which centres and restricts children to a maximum width</p></LayoutWidthContainer>

export const Basic = Template.bind({})
Basic.args = {
    className: 'u-border-dashed u-border-4'
}

