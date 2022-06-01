import '../UI/scss/screen.scss'

import React, { useEffect, useState, useRef, lazy, Suspense } from 'react'
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
import { ThemesContext, FormsContext } from '@contexts/index'

const CustomApp = ({ Component, pageProps }) => {

    const activeRequests: any = useRef([]);
    const [isLoading, setIsLoading] = useState(false);

    const router = useRouter()
    const { errors, layoutId, csrfToken } = pageProps

    const formsContextConfig: Record<string, any> = {
        csrfToken: csrfToken,
        templates: formConfigs,
    }

    const themesContextConfig: Record<string, any> = {
        themes: themes,
    }

    const hasServerError: boolean =
        errors?.filter((error) =>
            Object.keys(error).filter((key) => Number(key) >= 500)
        ).length > 0
    let hasFormErrors: boolean = false
    let headTitle: string = pageProps.pageTitle || pageProps.contentText?.title;

    const Loading = lazy(() => import('../components/Loading/index'))

    useEffect(() => {

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
        });

        /*
 * Listen for fetch events and render loading component on long running requests
 */
        (function (proxy, fetch): void {

            proxy.fetch = function (url: string) {
                var out = fetch.apply(this, arguments);

                activeRequests.current.push(url);
                setIsLoading(true);

                out.then(() => {

                    activeRequests.current = activeRequests.current.filter(item => item !== url);
                    !activeRequests.current.length && setIsLoading(false);

                });

                return out;
            }

        }(window, window.fetch))
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
        return (
            <ThemesContext.Provider value={themesContextConfig}>
                <FormsContext.Provider value={formsContextConfig}>
                    <StandardLayout {...pageProps} user={null}>
                        <ErrorPage statusCode={500} />
                    </StandardLayout>
                    {isLoading &&
                        <Suspense fallback={() => { }}>
                            <Loading />
                        </Suspense>
                    }
                </FormsContext.Provider>
            </ThemesContext.Provider>
        )
    }

    if (layoutId === layoutIds.GROUP) {
        return (
            <ThemesContext.Provider value={themesContextConfig}>
                <FormsContext.Provider value={formsContextConfig}>
                    <GroupLayout {...pageProps}>
                        <Head>
                            <title>{headTitle}</title>
                        </Head>
                        <Component {...pageProps} key={router.asPath} />
                    </GroupLayout>
                    {isLoading &&
                        <Suspense fallback={() => { }}>
                            <Loading />
                        </Suspense>
                    }
                </FormsContext.Provider>
            </ThemesContext.Provider>
        )
    }

    if (layoutId === layoutIds.ADMIN) {
        return (
            <ThemesContext.Provider value={themesContextConfig}>
                <FormsContext.Provider value={formsContextConfig}>
                    <AdminLayout {...pageProps}>
                        <Head>
                            <title>{headTitle}</title>
                        </Head>
                        <Component {...pageProps} key={router.asPath} />
                    </AdminLayout>
                    {isLoading &&
                        <Suspense fallback={() => { }}>
                            <Loading />
                        </Suspense>
                    }
                </FormsContext.Provider>
            </ThemesContext.Provider>
        )
    }

    return (
        <ThemesContext.Provider value={themesContextConfig}>
            <FormsContext.Provider value={formsContextConfig}>
                <StandardLayout {...pageProps}>
                    <Head>
                        <title>{headTitle}</title>
                    </Head>
                    <Component {...pageProps} key={router.asPath} />
                </StandardLayout>
                {isLoading &&
                    <Suspense fallback={() => { }}>
                        <Loading />
                    </Suspense>
                }
            </FormsContext.Provider>
        </ThemesContext.Provider>
    )
}

CustomApp.getInitialProps = async (appContext) => {
    const appProps = await App.getInitialProps(appContext)

    return { ...appProps }
}

export default CustomApp
