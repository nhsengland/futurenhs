import { CommentLike } from '@services/getGroupDiscussionCommentLikes'

export interface Props {
    /**
     * Id of entity being liked
     */
    targetId: string
    /**
     * Overrides default thumbs up icon
     */
    iconName?: string
    /**
     * Renders how many likes the entity has
     */
    likeCount: number
    /**
     * Checks if user has already liked the entity
     */
    isLiked?: boolean
    /**
     * Controls whether to display like button or unclickable icon on initial render
     */
    shouldEnable?: boolean
    /**
     * Function that is triggered when like button is clicked e.g API call
     */
    likeAction: any
    /**
     * Controls visible label counts and aria-labels
     */
    text: {
        countSingular: string
        countPlural: string
        like: string
        removeLike: string
    }
    likes: Array<CommentLike>
    className?: string
}
