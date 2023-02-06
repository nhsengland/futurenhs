import React from 'react'
import { Props } from './interfaces'

import { Comment } from './index'

export default {
    title: 'Comment',
    component: Comment,
    argTypes: {
        csrfToken: {
            control: { type: '' },
        },
        image: {
            control: { type: '' },
        },
        initialErrors: {
            control: { type: '' },
        },
    },
}

const Template = (args) => <Comment {...args} />

const basicArgs: Props = {
    commentId: 'commentId',
    csrfToken: '1234',
    replySubmitAction: () => true,
    likeAction: () => true,
    text: {
        userName: 'Stephen Stephenson',
        initials: 'SS',
        body: 'This is a comment',
    },
    userProfileLink: '/',
    date: '9 May 2022',
    shouldEnableReplies: false,
    shouldEnableLikes: true,
    likes: [
        {
            id: '1',
            createdByThisUser: undefined,
            createdAtUtc: undefined,
            firstRegistered: {
                atUtc: undefined,
                by: {
                    id: undefined,
                    name: undefined,
                    slug: undefined,
                    image: undefined,
                },
            },
        },
    ],
    isLiked: false,
}

export const Basic = Template.bind({})
Basic.args = Object.assign({}, basicArgs)

export const Reply = Template.bind({})
Reply.args = Object.assign({}, basicArgs, {
    shouldEnableReplies: true,
})

export const OriginComment = Template.bind({})
OriginComment.args = Object.assign({}, basicArgs, {
    originComment: {
        commentId: 'originId',
        createdBy: {
            text: {
                userName: 'John Johnson',
            },
        },
        text: {
            body: 'Origin comment',
        },
    },
})
