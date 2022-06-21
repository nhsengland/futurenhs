import { Image } from './image'

export interface User {
    id: string
    text: {
        userName: string
    }
    image?: Image
}
