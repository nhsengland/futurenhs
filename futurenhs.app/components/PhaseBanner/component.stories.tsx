import { PhaseBanner} from './index'

export default {
    title: 'PhaseBanner',
    component: PhaseBanner
}

const Template = (args) => <PhaseBanner {...args} />

export const Basic = Template.bind({})
Basic.args = {
    text: {
        tag: 'beta',
        body: 'This is a new service – your <a href="https://forms.office.com/r/0ENi61JEu8">feedback</a> will help us to improve it.'
    }
}