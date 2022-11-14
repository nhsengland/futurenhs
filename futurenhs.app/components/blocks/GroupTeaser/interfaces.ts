import { GroupInvite } from '@appTypes/group'
import { Image } from '@appTypes/image'
import { User } from '@appTypes/user'

export interface Props {
    text: {
        mainHeading?: string
        strapLine?: string
    }
    groupId?: string
    groupInvite?: GroupInvite
    refreshGroupInvites?: () => void
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
