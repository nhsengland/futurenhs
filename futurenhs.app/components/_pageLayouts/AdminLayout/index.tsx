import Head from 'next/head'

import { PageBody } from '@components/PageBody'
import { StandardLayout } from '@components/_pageLayouts/StandardLayout'

import { Props } from './interfaces'

export const AdminLayout: (props: Props) => JSX.Element = ({
    contentText,
    user,
    actions,
    children,
    pageTitle,
}) => {
    const { metaDescription, title, mainHeading } = contentText ?? {}

    const generatedIds: any = {}

    const generatedClasses = {}

    return (
        <StandardLayout user={user} actions={actions} className="u-bg-theme-3">
            <Head>
                <meta name="description" content={metaDescription} />
            </Head>
            <div className="u-pt-10 tablet:u-px-4">
                <h1 className="u-m-0">{mainHeading}</h1>
            </div>
            {children}
        </StandardLayout>
    )
}
