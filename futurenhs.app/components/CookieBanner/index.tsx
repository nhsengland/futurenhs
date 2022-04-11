import Cookies from 'js-cookie'
import classNames from 'classnames'
import { useState, useEffect } from 'react'

import { LayoutWidthContainer } from '@components/LayoutWidthContainer'
import { RichText } from '@components/RichText'
import { cookiePreferences } from '@constants/cookies'

import { Props } from './interfaces'

export const CookieBanner: (props: Props) => JSX.Element = ({
    cookieName = cookiePreferences.COOKIE_NAME,
    cookieAcceptValue = cookiePreferences.ACCEPTED,
    cookieRejectValue = cookiePreferences.REJECTED,
    expiresInDays = 90,
    text = {
        title: 'Cookies on the FutureNHS website',
        body: [
            "We've put some small files called cookies on your device to make our site work.",
            "We'd also like to use analytics cookies. These send information about how our site is used to services called Hotjar and Google Analytics. We use this information to improve our site.",
            'Let us know if this is OK. We\'ll use a cookie to save your choice. You can <a href="/cookies">read more about our cookies</a> before you choose.',
        ],
        accept: "I'm OK with analytics cookies",
        reject: 'Do not use analytics cookies',
    },
}) => {
    const [shouldRenderBanner, setShouldRenderBanner] = useState(false)

    const { body, accept, reject, title } = text ?? {}

    useEffect(() => {
        const hasCookies = Cookies.get(cookieName)

        if (!hasCookies) setShouldRenderBanner(true)
    }, [])

    const generatedClasses: any = {
        wrapper: classNames('u-py-6', 'c-cookie-banner'),
        paragraphs: classNames('u-mb-6'),
        heading: classNames('nhsuk-heading-l'),
        button: classNames(
            'c-button',
            'u-mr-6',
            'u-mt-6',
            'c-cookie-banner_button'
        ),
    }

    const handleAcceptCookies = () => {
        Cookies.set(cookieName, cookieAcceptValue, { expires: expiresInDays })
        setShouldRenderBanner(false)
        window.location.reload()
    }

    const handleRejectCookies = () => {
        Cookies.set(cookieName, cookieRejectValue, { expires: expiresInDays })
        setShouldRenderBanner(false)
        window.location.reload()
    }

    if (shouldRenderBanner) {
        return (
            <LayoutWidthContainer>
                <div className={generatedClasses.wrapper}>
                    <h2 className={generatedClasses.heading}>{title}</h2>
                    {body?.map((paragraph, index) => {
                        return (
                            <RichText
                                wrapperElementType="p"
                                bodyHtml={paragraph}
                                key={index}
                            />
                        )
                    })}
                    <button
                        className={generatedClasses.button}
                        onClick={handleAcceptCookies}
                    >
                        {accept}
                    </button>
                    <button
                        className={generatedClasses.button}
                        onClick={handleRejectCookies}
                    >
                        {reject}
                    </button>
                </div>
            </LayoutWidthContainer>
        )
    }

    return null
}
