import { Image } from '@appTypes/image'

export interface Props {
    id?: string
    children: any
    image?: Image
    clickableHref?: string
    className?: string
}
