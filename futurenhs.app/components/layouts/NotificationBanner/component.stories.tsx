import StandardLayout from '@components/layouts/pages/StandardLayout'
import { NotificationBanner } from './index'

export default {
    title: 'NotificationBanner',
    component: NotificationBanner,
}

const Template = (args) => <NotificationBanner {...args} />

export const Basic = Template.bind({})
Basic.args = {
    text: {
        heading: 'Important',
        main: 'An example notification',
    },
}

export const Success = Template.bind({})
Success.args = {
    text: {
        main: 'Your profile has been updated',
    },
}
