import { Image } from '@appTypes/image'

export interface Props {
    /**
     * Image to be rendered
     */
    image: Image
    /**
     * Renders initials if an image is not provided
     */
    initials: string
    className?: string
}
