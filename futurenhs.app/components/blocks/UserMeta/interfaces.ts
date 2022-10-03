import { Image } from '@appTypes/image'

export interface Props {
    image: Image
    text: {
        initials: string
    }
    children: any
    className?: string
}
