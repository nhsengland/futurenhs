import { GetServerSideProps } from 'next'
import Head from 'next/head'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withTextContent } from '@hofs/withTextContent'
import { selectPageProps } from '@selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { PageBody } from '@components/PageBody'
import { Page } from '@appTypes/page'

export interface Props extends Page {}

/**
 * Home page template
 */
export const HomePage: (props: Props) => JSX.Element = ({ contentText }) => {
    const { title, metaDescription, mainHeading } = contentText ?? {}

    return (
        <>
            <Head>
                <meta name="description" content={metaDescription} />
            </Head>
            <LayoutColumnContainer>
                <LayoutColumn>
                    <PageBody>
                        <h1 className="nhsuk-heading-xl">{mainHeading}</h1>
                    </PageBody>
                </LayoutColumn>
            </LayoutColumnContainer>
        </>
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
            routeId: '749bd865-27b8-4af6-960b-3f0458f8e92f',
        },

        [withUser, withRoutes, withTextContent],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get data from request context
             */
            const props: Partial<Props> = selectPageProps(context)

            return {
                redirect: {
                    permanent: false,
                    destination: props.routes.groupsRoot,
                },
            }
        }
    )

/**
 * Export page template
 */
export default HomePage
