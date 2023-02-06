import { cleanup, render, screen } from '@jestMocks/index'

import { Props } from './interfaces'
import { Comment } from './index'

describe('Comment', () => {
    const props: Props = {
        commentId: 'commentId',
        csrfToken: '1234',
        replySubmitAction: null,
        likeAction: null,
        text: {
            userName: 'Stephen Stephenson',
            initials: 'SS',
            body: 'This is a comment',
        },
        userProfileLink: '/',
        date: '7 July 2022',
        shouldEnableReplies: false,
        shouldEnableLikes: true,
        isLiked: false,
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
    }

    it('Renders correctly', () => {
        render(<Comment {...props} />)

        expect(screen.getAllByText('This is a comment').length).toBe(1)
    })

    it('Conditionally renders origin comment link', () => {
        render(<Comment {...props} />)

        expect(screen.queryByText(/In response to/g)).toBeNull()

        cleanup()

        const propsCopy: Props = Object.assign({}, props, {
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

        render(<Comment {...propsCopy} />)

        expect(screen.getAllByText(/In response to/g).length).toBe(1)
    })

    it('Conditionally renders reply component', () => {
        render(<Comment {...props} />)

        expect(screen.queryByText('Reply')).toBeNull()

        cleanup()

        const propsCopy: Props = Object.assign({}, props, {
            shouldEnableReplies: true,
        })

        render(<Comment {...propsCopy} />)

        expect(screen.getAllByText('Reply').length).toBeGreaterThan(0)
    })
})
