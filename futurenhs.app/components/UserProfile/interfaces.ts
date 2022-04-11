import { Image } from '@appTypes/image'
import { Member } from '@appTypes/member'

export interface Props {
    profile: Member
    text: {
        heading: string
        firstNameLabel: string
        lastNameLabel: string
        pronounsLabel: string
        emailLabel: string
    }
    image?: Image
    children?: any
    className?: string
    headingLevel?: number
}
