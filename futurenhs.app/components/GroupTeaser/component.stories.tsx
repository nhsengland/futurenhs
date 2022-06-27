import React from 'react'
import { GroupTeaser } from './index'
import { useAssetPath } from '@hooks/useAssetPath'

const exampleImageSrc: string = useAssetPath('/images/example-group-image.svg')

export default {
    title: 'GroupTeaser',
    component: GroupTeaser,
    argTypes: {
        themeId: {
            options: [
                '36d49305-eca8-4176-bfea-d25af21469b9',
                '9a3c911b-c3d3-4f58-a32a-d541e0f5bf56',
                '5053a8c6-ea4d-4125-9dc3-475e3e931fee',
            ],
            control: { type: 'radio' },
        },
    },
}

const Template = (args) => <GroupTeaser {...args} />

export const Basic = Template.bind({})
Basic.args = {
    totalMemberCount: 57,
    totalDiscussionCount: 629,
    text: {
        mainHeading: 'Group name',
        strapLine: 'Group strapline text content',
    },
    image: {
        src: exampleImageSrc,
        height: 180,
        width: 180,
        altText: 'Group image',
    },
    themeId: '36d49305-eca8-4176-bfea-d25af21469b9',
}

export const NoImage = Template.bind({})
NoImage.args = {
    totalMemberCount: 57,
    totalDiscussionCount: 629,
    text: {
        mainHeading: 'Group name',
        strapLine: 'Group strapline text content',
    },
    image: null,
    themeId: '36d49305-eca8-4176-bfea-d25af21469b9',
}

export const Theme2 = Template.bind({})
Theme2.args = {
    totalMemberCount: 57,
    totalDiscussionCount: 629,
    text: {
        mainHeading: 'Group name',
        strapLine: 'Group strapline text content',
    },
    image: {
        src: exampleImageSrc,
        height: 180,
        width: 180,
        altText: 'Group image',
    },
    themeId: '9a3c911b-c3d3-4f58-a32a-d541e0f5bf56',
}

export const Theme3 = Template.bind({})
Theme3.args = {
    totalMemberCount: 57,
    totalDiscussionCount: 629,
    text: {
        mainHeading: 'Group name',
        strapLine: 'Group strapline text content',
    },
    image: {
        src: exampleImageSrc,
        height: 180,
        width: 180,
        altText: 'Group image',
    },
    themeId: '5053a8c6-ea4d-4125-9dc3-475e3e931fee',
}
