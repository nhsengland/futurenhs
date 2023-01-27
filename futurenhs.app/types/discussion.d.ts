import { User } from './user'

export interface Discussion {
    discussionId?: string
    responseCount?: number
    viewCount?: number
    createdBy: Partial<User>
    created: string
    modifiedBy?: Partial<User>
    modified?: string
    isSticky?: boolean
    text: {
        title: string
        body?: string
    }
}

export interface DiscussionComment {
    commentId: string
    originCommentId?: string
    createdBy?: Partial<User>
    createdBy?: Partial<User>
    created?: string
    modifiedBy?: Partial<User>
    modified?: string
    replyCount?: number
    likeCount?: number
    isLiked?: boolean
    likes: Array<CommentLike>
    text: {
        body?: string
    }
    replies?: Array<DiscussionComment>
}
