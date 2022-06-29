import { useRef, useState, useEffect } from 'react'
import classNames from 'classnames'

import { Props } from './interfaces'
import { RichText } from '@components/RichText'
import { notifications } from '@constants/notifications'
import { Heading } from '@components/Heading'

export const NotificationBanner: (props: Props) => JSX.Element = ({
    id,
    text,
    className,
    headingLevel = 2,
}) => {
    const wrapperRef: any = useRef(null)

    const { heading, main } = text ?? {}
    const notificationHeading: string = heading
        ? heading
        : notifications.SUCCESS

    const isAlert: boolean = notificationHeading === notifications.SUCCESS

    const generatedClasses: any = {
        wrapper: classNames(
            'c-notification-banner',
            { ['c-notification-banner--success']: isAlert },
            className
        ),
        header: classNames('c-notification-banner__header'),
        title: classNames('c-notification-banner__title'),
        body: classNames('c-notification-banner__content'),
        bodyHeading: classNames('c-notification-banner__heading'),
    }

    const generatedIds: any = {
        title: `${id}-notification-banner-title`,
    }

    useEffect(() => {
        const wrapper: HTMLElement = wrapperRef?.current
        wrapper?.setAttribute('tabIndex', '-1')
        wrapper?.classList?.add('focus:u-outline-none')
        wrapper?.focus()
        wrapper?.addEventListener('blur', () => {
            wrapper?.removeAttribute('tabIndex')
        })
    }, [wrapperRef.current])

    return (
        <div
            className={generatedClasses.wrapper}
            role={isAlert ? 'alert' : 'region'}
            aria-labelledby={generatedIds.title}
            ref={wrapperRef}
        >
            <div className={generatedClasses.header}>
                <Heading
                    level={headingLevel}
                    className={generatedClasses.title}
                    id={generatedIds.title}
                >
                    {notificationHeading}
                </Heading>
            </div>
            <div className={generatedClasses.body}>
                <div className={generatedClasses.bodyHeading}>
                    <RichText bodyHtml={main} />
                </div>
            </div>
        </div>
    )
}
