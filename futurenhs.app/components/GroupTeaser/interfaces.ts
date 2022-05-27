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
    className?: string
}
