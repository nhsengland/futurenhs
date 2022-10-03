import Head from 'next/head'
import StandardLayout from '@components/layouts/pages/StandardLayout'
import { GenericPageTextContent } from '@appTypes/content'
import { Theme } from '@appTypes/theme'
export interface Props {
    shouldRenderSearch?: boolean
    shouldRenderUserNavigation?: boolean
    shouldRenderPhaseBanner?: boolean
    shouldRenderBreadCrumb?: boolean
    shouldRenderMainNav?: boolean
    user?: any
    actions?: any
    theme?: Theme
    className?: string
    contentText?: GenericPageTextContent
    children?: any
    pageTitle?: string
}

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
            {mainHeading && (
                <div className="u-pt-10 tablet:u-px-4">
                    <h1 className="u-m-0">{mainHeading}</h1>
                </div>
            )}
            {children}
        </StandardLayout>
    )
}
