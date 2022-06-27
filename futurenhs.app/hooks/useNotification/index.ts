
import { notifications } from '@constants/notifications'
import { Notification } from '@components/NotificationBanner/interfaces'

export const useNotification = (notificationsContext: any, notificationBody: string, heading?: string): void => {

    const newNotification: Notification = {
        heading: heading ? heading : notifications.IMPORTANT,
        main: notificationBody
    }

    notificationsContext.setNotifications((currentNotifications) => [...currentNotifications, newNotification])

}