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
    totalFileCount?: number
    owner?: Partial<User>
    isDeleted?: boolean
    isPublic?: boolean
    invite?: InviteDetails
}

export interface GroupInvitedBy {
    name: string
}

type InviteDetails = {
    id: string
    membershipUser_Id: string
    groupId: string
    rowVersion: string
    user: string
    createdAtUTC: string
    createdBy: string
}

export interface GroupMember extends Member {}
