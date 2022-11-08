import { Image } from '@appTypes/image'
import { User } from '@appTypes/user'

export interface Props {
    text: {
        mainHeading?: string
        strapLine?: string
    }
    groupId?: string
    themeId?: string
    totalDiscussionCount?: number
    totalMemberCount?: number
    image?: Image
    headingLevel?: number
    className?: string
    isPublic?: boolean
    isSignUp?: boolean
    isPending?: boolean
    user?: User
}
