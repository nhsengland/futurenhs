import { LayoutColumnContainer } from '../LayoutColumnContainer';
import { LayoutColumn } from './index';

export default {
    title: 'LayoutColumn',
    component: LayoutColumn
}

const Template = (args) => <div className="u-p-14"><LayoutColumnContainer className="u-bg-theme-3">
    <LayoutColumn {...args}><p className="u-h-14 u-p-4">Child content</p></LayoutColumn>
</LayoutColumnContainer></div>

export const Basic = Template.bind({})
Basic.args = {
    className: 'u-border-dashed u-border-4'
}

