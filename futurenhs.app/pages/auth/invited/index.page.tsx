import { GetServerSideProps } from 'next'
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
import { getGroupsByInvite } from '@services/getGroupsByInvite'
import { Group, GroupInvitedBy } from '@appTypes/group'
import { GroupTeaser } from '@components/blocks/GroupTeaser'
declare interface ContentText extends GenericPageTextContent {}

export interface Props extends Page {
    contentText: ContentText
    group: Group
    invitedBy: GroupInvitedBy
}

const AuthInvitedPage: (props: Props) => JSX.Element = ({
    contentText,
    group,
    invitedBy,
}) => {
    const { bodyHtml, mainHeading, secondaryHeading } = contentText
    /**
     * Render
     */
    return (
        <PageBody className="tablet:u-px-0">
            <LayoutColumnContainer justify="centre">
                <LayoutColumn tablet={8} desktop={6}>
                    <h1 className="nhsuk-heading-xl">{mainHeading}</h1>

                    {group ? (
                        <h4 className="nhsuk-heading-md">
                            {secondaryHeading
                                .replace('%GROUPNAME%', group.text.mainHeading)
                                .replace('%INVITEDBY%', invitedBy.name)}
                            <GroupTeaser {...group} isSignUp />{' '}
                        </h4>
                    ) : null}

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
                    const { invitedBy, group } = res.data
                    props.invitedBy = invitedBy
                    props.group = group
                    return handleSSRSuccessProps({ props, context })
                } catch (error) {
                    return handleSSRErrorProps({ props, error })
                }
            }
            /**
             * Hide breadcrumbs
             */
            ;(props as any).breadCrumbList = []

            /**
             * Return data to page template
             */
            return handleSSRSuccessProps({ props, context })
        }
    )

export default AuthInvitedPage
