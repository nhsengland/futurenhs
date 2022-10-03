import { notifications } from '@constants/notifications'
import { Notification } from '@components/layouts/NotificationBanner/interfaces'

export interface UseNotificationOptions {
    notificationsContext: any
    shouldClearQueue?: boolean
    text?: {
        heading?: string
        body: string
    }
}

export const useNotification = ({
    notificationsContext,
    shouldClearQueue,
    text,
}: UseNotificationOptions): void => {
    if (shouldClearQueue) {
        notificationsContext.setNotifications([])

        return
    }

    const { heading, body } = text ?? {}

    const newNotification: Notification = {
        heading: heading ? heading : notifications.IMPORTANT,
        main: body,
    }

    notificationsContext.setNotifications((currentNotifications) => [
        ...currentNotifications,
        newNotification,
    ])
}
