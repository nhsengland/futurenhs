import { Image } from './image'
import { Member } from '@appTypes/member'
import { GroupsPageTextContent } from '@appTypes/content'

export interface Group {
    text: GroupsPageTextContent
    groupId?: string
    themeId?: string
    imageId?: string
    image?: Image
    totalDiscussionCount?: number
    totalMemberCount?: number
    owner?: Partial<User>
    isDeleted?: boolean
    isPublic?: boolean
    invite?: GroupInvite
}

export interface GroupInvitedBy {
    name: string
}

type GroupInvite = {
    id: string
    membershipUser_Id: string
    groupId: string
    rowVersion: string
}

export interface GroupMember extends Member {}
