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
import { Group, GroupInvitedBy } from '@appTypes/group'
import { GroupTeaser } from '@components/blocks/GroupTeaser'
import { routes } from '@constants/routes'
import { getGroupsByInvite } from '@services/getGroupsByInvite'
declare interface ContentText extends GenericPageTextContent {}

export interface Props extends Page {
    contentText: ContentText
    group: Group | null
    invitedBy: GroupInvitedBy | null
    b2cUrl: string
}

const AuthInvitedPage: (props: Props) => JSX.Element = ({
    contentText,
    group,
    invitedBy,
    b2cUrl,
}) => {
    const { bodyHtml, mainHeading, secondaryHeading } = contentText ?? {}

    /**
     * Render
     */

    const GroupSubheader = () => {
        const subHeader = secondaryHeading
            .replace('%GROUPNAME%', group?.text?.mainHeading)
            .replace('%INVITEDBY%', invitedBy?.name)
        return group && invitedBy ? (
            <h4 className="nhsuk-heading-md">
                {subHeader}
                <GroupTeaser {...group} isSignUp />
            </h4>
        ) : null
    }
    return (
        <PageBody className="tablet:u-px-0">
            <LayoutColumnContainer justify="centre">
                <LayoutColumn tablet={8} desktop={6}>
                    <h1 className="nhsuk-heading-xl">{mainHeading}</h1>
                    <GroupSubheader />
                    <RichText bodyHtml={bodyHtml} />
                    <a className="c-button" href={b2cUrl}>
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
                const callbackUrl: string = `${process.env.APP_URL}/api/auth/signin/azure-ad-b2c`
                const b2cUrl = `https://${process.env.AZURE_AD_B2C_TENANT_NAME}.b2clogin.com/${process.env.AZURE_AD_B2C_TENANT_NAME}.onmicrosoft.com/${process.env.AZURE_AD_B2C_SIGNUP_USER_FLOW}/oauth2/v2.0/authorize?client_id=${process.env.AZURE_AD_B2C_CLIENT_ID}&scope=offline_access%20openid&response_type=code&redirect_uri=${callbackUrl}`
                try {
                    const res = await getGroupsByInvite({ id })
                    const { invitedBy, group } = res.data
                    props.invitedBy = invitedBy
                    props.group = group
                    props.b2cUrl = b2cUrl
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
