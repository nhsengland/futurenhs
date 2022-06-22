import React from 'react'
import { UserMeta } from './index'
import { useAssetPath } from '@hooks/useAssetPath';

const exampleImageSrc: string = useAssetPath('/images/example-profile-image.svg');

export default {
    title: 'UserMeta',
    component: UserMeta,
    argTypes: {
        image: {
            control: { type: '' },
        },
    },
}

const Template = (args) => <UserMeta {...args}>
    <span className="u-border-dashed u-border-4 u-h-full u-p-2">Child content</span>
</UserMeta>

export const Basic = Template.bind({})
Basic.args = {
    text: {
        initials: 'AE',
    },
    image: {
        src: exampleImageSrc,
        altText: 'Image of user',
        height: '250px',
        width: '250px'
    }
}