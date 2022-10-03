import { LayoutWidthContainer } from './index'

export default {
    title: 'LayoutWidthContainer',
    component: LayoutWidthContainer,
}

const Template = (args) => (
    <LayoutWidthContainer {...args}>
        <p className="u-m-4"></p>
    </LayoutWidthContainer>
)

export const Basic = Template.bind({})
Basic.args = {
    className: 'u-border-dashed u-border-4',
}
