import { GetServerSideProps } from 'next'
import { getSession } from 'next-auth/react'
import { useRef, useState } from 'react'
import { useRouter } from 'next/router'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { selectPageProps } from '@helpers/selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { PageBody } from '@components/layouts/PageBody'
import { RichText } from '@components/generic/RichText'
import { GenericPageTextContent } from '@appTypes/content'
import { Page } from '@appTypes/page'
import { redirect } from 'next/dist/server/api-utils'
import { getGroupsByInvite } from '@services/getGroupsByInvite'
declare interface ContentText extends GenericPageTextContent {}

export interface Props extends Page {
    contentText: ContentText
    group: string
}

const AuthRegisterPage: (props: Props) => JSX.Element = ({ contentText }) => {
    const router = useRouter()

    const { mainHeading, bodyHtml } = contentText ?? {}

    /**
     * Render
     */
    return (
        <PageBody className="tablet:u-px-0">
            <LayoutColumnContainer justify="centre">
                <LayoutColumn tablet={8} desktop={6}>
                    <h1 className="nhsuk-heading-xl">{mainHeading}</h1>
                    <RichText bodyHtml={bodyHtml} />
                    <a className="c-button u-w-full" href={'/auth/signin'}>
                        Sign Up
                    </a>
                </LayoutColumn>
            </LayoutColumnContainer>
        </PageBody>
    )
}

/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (
    context: GetServerSidePropsContext
) =>
    await pipeSSRProps(
        context,
        {
            routeId: '178d565d-00e6-4bd9-8bb0-a41a7b75aad6',
        },
        [withRoutes, withTextContent],
        async (context: GetServerSidePropsContext) => {
            const props: Partial<Props> = selectPageProps(context)
            const { id } = context.query
            if (id && typeof id === 'string') {
                try {
                    const res = await getGroupsByInvite({ id })
                    // res.group = 'Test Group'
                    return handleSSRSuccessProps({ props, context })
                } catch (e) {}
            } else {
                return { redirect: {} }
            }
            /**
             * Hide breadcrumbs
             */
            ;(props as any).breadCrumbList = []

            /**
             * Return data to page template
             */
        }
    )

export default AuthRegisterPage
