import App from 'next/app'
import { useRouter } from 'next/router'
import Head from 'next/head'
import ErrorPage from '@pages/500.page'

import { StandardLayout } from '@components/_pageLayouts/StandardLayout'
import { GroupLayout } from '@components/_pageLayouts/GroupLayout'
import { AdminLayout } from '@components/_pageLayouts/AdminLayout'
import { layoutIds } from '@constants/routes'

import '../UI/scss/screen.scss'

const CustomApp = ({ Component, pageProps }) => {
    const router = useRouter()
    const { errors, layoutId } = pageProps

    const hasServerError: boolean =
        errors?.filter((error) =>
            Object.keys(error).filter((key) => Number(key) >= 500)
        ).length > 0
    let hasFormErrors: boolean = false
    let headTitle: string = pageProps.pageTitle || pageProps.contentText?.title

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
            <StandardLayout {...pageProps} user={null}>
                <ErrorPage statusCode={500} />
            </StandardLayout>
        )
    }

    if (layoutId === layoutIds.GROUP) {
        return (
            <GroupLayout {...pageProps}>
                <Head>
                    <title>{headTitle}</title>
                </Head>
                <Component {...pageProps} key={router.asPath} />
            </GroupLayout>
        )
    }

    if (layoutId === layoutIds.ADMIN) {
        return (
            <AdminLayout {...pageProps}>
                <Head>
                    <title>{headTitle}</title>
                </Head>
                <Component {...pageProps} key={router.asPath} />
            </AdminLayout>
        )
    }

    return (
        <StandardLayout {...pageProps}>
            <Head>
                <title>{headTitle}</title>
            </Head>
            <Component {...pageProps} key={router.asPath} />
        </StandardLayout>
    )
}

CustomApp.getInitialProps = async (appContext) => {
    const appProps = await App.getInitialProps(appContext)

    return { ...appProps }
}

export default CustomApp
