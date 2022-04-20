import React from 'react'
import { Like } from './index'

export default {
    title: 'Like',
    component: Like,
    argTypes: {
        iconName: {
          options: ['icon-thumbs-up', 'icon-star', 'icon-tick', 'icon-plus-circle', 'icon-like-fill'],
          control: { type: 'select' },
        },
      },
}

const Template = (args) => <Like {...args} />

export const Basic = Template.bind({})
Basic.args = {
    shouldEnable: true,
    likeCount: 0,
    isLiked: false,
    text: {
        countSingular: 'like',
        countPlural: 'likes',
        like: 'like',
        removeLike: 'Remove like',
    },
    likeAction: () => true
}

export const Disabled = Template.bind({})
Disabled.args = {
    shouldEnable: false,
    likeCount: 5,
    isLiked: true,
    text: {
        countSingular: 'like',
        countPlural: 'likes',
        like: 'like',
        removeLike: 'Remove like',
    },
    likeAction: () => true
}

export const Star = Template.bind({})
Star.args = {
    iconName: 'icon-star',
    shouldEnable: true,
    likeCount: 5,
    isLiked: false,
    text: {
        countSingular: 'star',
        countPlural: 'stars',
        like: 'Add star',
        removeLike: 'Remove star',
    },
    likeAction: () => true
}