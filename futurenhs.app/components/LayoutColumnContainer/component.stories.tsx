import { LayoutColumnContainer } from './index';
import { LayoutColumn } from '../LayoutColumn';

export default {
    title: 'LayoutColumnContainer',
    component: LayoutColumnContainer
}

const Template = (args) => <div className="u-p-14"><LayoutColumnContainer {...args}>
    <LayoutColumn tablet={3} className="u-bg-theme-3"><p className="u-h-14 u-p-4">Column 1</p></LayoutColumn>
    <LayoutColumn tablet={3} className="u-bg-theme-3"><p className="u-h-14 u-p-4">Column 2</p></LayoutColumn>
</LayoutColumnContainer></div>

export const Basic = Template.bind({})
Basic.args = {
    className: 'u-border-dashed u-border-4'
}

