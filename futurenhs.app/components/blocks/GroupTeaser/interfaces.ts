import { Image } from '@appTypes/image'

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
}
