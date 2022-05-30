import { Avatar } from './index'
import { defaultGroupLogos } from '@constants/icons'

export default {
    title: 'Avatar',
    component: Avatar,
    argTypes: {
        image: {
            control: { type: '' },
        },
    },
}

const Template = (args) => <Avatar {...args} />

export const Basic = Template.bind({})
Basic.args = {
    image: defaultGroupLogos.large,
    initials: 'AA',
}

export const NoImage = Template.bind({})
NoImage.args = {
    image: null,
    initials: 'AA',
    className: 'u-w-36 u-h-36',
}
