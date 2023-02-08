import { Image } from '@appTypes/image'
import { DiscussionComment } from '@appTypes/discussion'
import { CommentLike } from '@services/getGroupDiscussionCommentLikes'

export interface Props {
    id?: string
    csrfToken: string
    initialErrors?: any
    /**
     * Stores id of comment to be sent with like/reply API calls
     */
    commentId: string
    /**
     * The comment that was replied to. If this is a top level comment, the discussion itself is referenced
     */
    originComment?: DiscussionComment
    /**
     * Renders child elements inside comment component. Typically used to render replies
     */
    children?: any
    /**
     * Profile image of user who created the comment
     */
    image?: Image
    /**
     * Includes users initials, username and the comment body
     */
    text: any
    /**
     * Sets the href for the users profile
     */
    userProfileLink: string
    /**
     * Formatted date the comment was added
     */
    date: string
    /**
     * Enables reply button and RTE
     */
    shouldEnableReplies?: boolean
    /**
     * Function to be called if there are validation issues when submitting a reply
     */
    replyValidationFailAction?: any
    /**
     * Function to be called upon submitting a reply
     */
    replySubmitAction: any
    /**
     * Determines whether likes are enabled
     */
    shouldEnableLikes?: boolean
    /**
     * Function to be called upon liking the comment
     */
    likeAction?: any
    /**
     * How many likes the comment has
     */
    likeCount?: number
    /**
     * Determines whether the user has liked the comment already
     */
    isLiked?: boolean
    likes: Array<CommentLike>
    className?: string
}
