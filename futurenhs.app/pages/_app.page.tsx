import '../UI/scss/screen.scss'

import React, { useEffect, useState, useRef } from 'react'
import App from 'next/app'
import { useRouter } from 'next/router'
import Head from 'next/head'
import ErrorPage from '@pages/500.page'

import { StandardLayout } from '@components/_pageLayouts/StandardLayout'
import { GroupLayout } from '@components/_pageLayouts/GroupLayout'
import { AdminLayout } from '@components/_pageLayouts/AdminLayout'
import { layoutIds } from '@constants/routes'
import formConfigs from '@formConfigs/index'
import { themes } from '@constants/themes'
import {
    ThemesContext,
    FormsContext,
    LoadingContext,
    NotificationsContext,
} from '@contexts/index'

const CustomApp = ({ Component, pageProps }) => {
    const activeRequests: any = useRef([])
    const [isLoading, setIsLoading] = useState(false)
    const [notifications, setNotifications] = useState(
        pageProps.notifications ?? []
    )

    const router = useRouter()
    const { errors, layoutId, csrfToken } = pageProps

    const formsContextConfig: Record<string, any> = {
        csrfToken: csrfToken,
        templates: formConfigs,
    }

    const themesContextConfig: Record<string, any> = {
        themes: themes,
    }

    const loadingContextConfig: Record<string, any> = {
        isLoading,
        text: {
            loadingMessage: '',
        },
    }

    const hasServerError: boolean =
        errors?.filter((error) =>
            Object.keys(error).filter((key) => Number(key) >= 500)
        ).length > 0
    let hasFormErrors: boolean = false
    let headTitle: string = pageProps.pageTitle || pageProps.contentText?.title

    useEffect(() => {
        /**
         * Empties notifications list on route change
         */
        router.events.on('routeChangeStart', () => {
            setNotifications([])
        })

        /*
         * This is needed for the focus to be moved back to the beginning of the page after
         * client-side routing by next-router.
         */
        router.events.on('routeChangeComplete', () => {
            document.body.setAttribute('tabIndex', '-1')
            document.body.focus()
        })

        document.body.addEventListener('blur', () => {
            document.body.removeAttribute('tabIndex')
        })

        /*
         * Listen for fetch events and render loading component on long running requests
         */
        ;(function (proxy, fetch): void {
            proxy.fetch = function (url: string) {
                const out = fetch.apply(this, arguments)

                activeRequests.current.push(url)
                setIsLoading(true)

                out.then(() => {
                    activeRequests.current = activeRequests.current.filter(
                        (item) => item !== url
                    )
                    !activeRequests.current.length && setIsLoading(false)
                })

                return out
            }
        })(window, window.fetch)
    }, [])

    if (pageProps.forms) {
        for (const form in pageProps.forms) {
            if (pageProps.forms?.[form]?.hasOwnProperty('errors')) {
                hasFormErrors = true
            }
        }
    }

    if (hasFormErrors) {
        headTitle = `Error: ${headTitle}`
    }

    if (hasServerError) {

        const errorsToRender: Record<string, any> = process.env.NEXT_PUBLIC_APP_DEBUG ? errors : null

        return (
            <NotificationsContext.Provider
                value={{ notifications, setNotifications }}
            >
                <ThemesContext.Provider value={themesContextConfig}>
                    <FormsContext.Provider value={formsContextConfig}>
                        <LoadingContext.Provider value={loadingContextConfig}>
                            <StandardLayout {...pageProps} user={null}>
                                <ErrorPage errors={errorsToRender} statusCode={500} />
                            </StandardLayout>
                        </LoadingContext.Provider>
                    </FormsContext.Provider>
                </ThemesContext.Provider>
            </NotificationsContext.Provider>
        )
    }

    if (layoutId === layoutIds.GROUP) {
        return (
            <NotificationsContext.Provider
                value={{ notifications, setNotifications }}
            >
                <ThemesContext.Provider value={themesContextConfig}>
                    <FormsContext.Provider value={formsContextConfig}>
                        <LoadingContext.Provider value={loadingContextConfig}>
                            <GroupLayout {...pageProps}>
                                <Head>
                                    <title>{headTitle}</title>
                                </Head>
                                <Component {...pageProps} key={router.asPath} />
                            </GroupLayout>
                        </LoadingContext.Provider>
                    </FormsContext.Provider>
                </ThemesContext.Provider>
            </NotificationsContext.Provider>
        )
    }

    if (layoutId === layoutIds.ADMIN) {
        return (
            <NotificationsContext.Provider
                value={{ notifications, setNotifications }}
            >
                <ThemesContext.Provider value={themesContextConfig}>
                    <FormsContext.Provider value={formsContextConfig}>
                        <LoadingContext.Provider value={loadingContextConfig}>
                            <AdminLayout {...pageProps}>
                                <Head>
                                    <title>{headTitle}</title>
                                </Head>
                                <Component {...pageProps} key={router.asPath} />
                            </AdminLayout>
                        </LoadingContext.Provider>
                    </FormsContext.Provider>
                </ThemesContext.Provider>
            </NotificationsContext.Provider>
        )
    }

    return (
        <NotificationsContext.Provider
            value={{ notifications, setNotifications }}
        >
            <ThemesContext.Provider value={themesContextConfig}>
                <FormsContext.Provider value={formsContextConfig}>
                    <LoadingContext.Provider value={loadingContextConfig}>
                        <StandardLayout {...pageProps}>
                            <Head>
                                <title>{headTitle}</title>
                            </Head>
                            <Component {...pageProps} key={router.asPath} />
                        </StandardLayout>
                    </LoadingContext.Provider>
                </FormsContext.Provider>
            </ThemesContext.Provider>
        </NotificationsContext.Provider>
    )
}

CustomApp.getInitialProps = async (appContext) => {
    const appProps = await App.getInitialProps(appContext)

    return { ...appProps }
}

export default CustomApp
