import { Avatar } from './index'
import { useAssetPath } from '@helpers/hooks/useAssetPath'

const exampleImageSrc: string = useAssetPath(
    '/images/example-profile-image.svg'
)

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
    image: {
        src: exampleImageSrc,
        altText: 'Image of user',
        height: '250px',
        width: '250px',
    },
    initials: 'AE',
    className: 'u-w-36 u-h-36',
}

export const NoImage = Template.bind({})
NoImage.args = {
    image: null,
    initials: 'AE',
    className: 'u-w-36 u-h-36',
}
