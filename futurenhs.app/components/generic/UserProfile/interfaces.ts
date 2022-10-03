import { Image } from '@appTypes/image'
import { Member } from '@appTypes/member'

export interface Props {
    /**
     * Member data, including name, email etc.
     */
    profile: Member
    /**
     * Configures page heading and labels for data fields
     */
    text: {
        heading: string
        firstNameLabel: string
        lastNameLabel: string
        pronounsLabel: string
        emailLabel: string
    }
    /**
     * Members profile image. If not included, members initials are displayed
     */
    image?: Image
    children?: any
    className?: string
    /**
     * Handles heading level for the main page heading - h2 by default
     */
    headingLevel?: number
}
