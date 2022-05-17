import {ErrorSummary} from './index'

export default {
    title: 'ErrorSummary',
    component: ErrorSummary
}

const Template = (args) => <ErrorSummary {...args} />

export const Basic = Template.bind({})
Basic.args = {
    errors: {
        name: 'Enter a name'
    },
    text: {
        body: 'There is a problem'
    }
}