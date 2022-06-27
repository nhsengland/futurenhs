import { notifications } from '@constants/notifications'

export interface Notification {
    heading?: string
    main?: string
}

export interface Props {
    id: number
    text: Notification
    className?: string
}
