import { GetServerSideProps } from 'next'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { layoutIds } from '@constants/routes'
import { actions } from '@constants/actions'
import { withUser } from '@helpers/hofs/withUser'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { selectPageProps } from '@helpers/selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { actions as actionConstants } from '@constants/actions'
import { routes } from '@constants/routes'
import { Link } from '@components/generic/Link'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { Card } from '@components/generic/Card'
import { Page } from '@appTypes/page'
import { SearchResult } from '@appTypes/search'
import { postApprovedDomain } from '@services/postApprovedDomain'

export interface Props extends Page {
    term: string | Array<string>
    resultsList: Array<SearchResult>
}

/**
 * Admin home dashboard template
 */
export const AdminHomePage: (props: Props) => JSX.Element = ({
    contentText,
    actions,
}) => {
    const generatedClasses = {}

    const submitDomainBan = async (e) => {
        e.preventDefault()
        const emailInput = e.target.querySelector('#emailAddress')
        try {
            const res = await postApprovedDomain({ domain: emailInput.value })
        } catch (e) {
            console.log(e)
        }
    }

    const shouldRenderUsersLink: boolean =
        actions.includes(actionConstants.SITE_ADMIN_MEMBERS_ADD) ||
        actions.includes(actionConstants.SITE_ADMIN_MEMBERS_EDIT) ||
        actions.includes(actionConstants.SITE_ADMIN_MEMBERS_DELETE)
    const shouldRenderGroupsLink: boolean =
        actions.includes(actionConstants.SITE_ADMIN_GROUPS_ADD) ||
        actions.includes(actionConstants.SITE_ADMIN_GROUPS_EDIT) ||
        actions.includes(actionConstants.SITE_ADMIN_GROUPS_DELETE)

    return (
        <>
            <LayoutColumnContainer className="c-page-body">
                {shouldRenderUsersLink && (
                    <LayoutColumn tablet={4}>
                        <Card clickableHref={routes.ADMIN_USERS}>
                            <h2 className="nhsuk-card__heading nhsuk-heading-m">
                                <Link href={routes.ADMIN_USERS}>
                                    Manage users
                                </Link>
                            </h2>
                        </Card>
                        <form onSubmit={(e) => submitDomainBan(e)}>
                            <label htmlFor="">email address</label>
                            <input id="emailAddress"></input>
                            <button type="submit">BAN</button>
                        </form>
                    </LayoutColumn>
                )}
                {shouldRenderGroupsLink && (
                    <LayoutColumn tablet={4}>
                        <Card clickableHref={routes.ADMIN_GROUPS}>
                            <h2 className="nhsuk-card__heading nhsuk-heading-m">
                                <Link href={routes.ADMIN_GROUPS}>
                                    Manage groups
                                </Link>
                            </h2>
                        </Card>
                    </LayoutColumn>
                )}
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
            routeId: '439794f2-9c58-4b6f-9fe8-d77a841e3055',
        },
        [withUser, withRoutes, withTextContent],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get data from request context
             */
            const props: Partial<Props> = selectPageProps(context)

            props.layoutId = layoutIds.ADMIN

            return {
                props,
                notFound: !props.actions.includes(actions.SITE_ADMIN_VIEW),
            }
        }
    )

/**
 * Export page template
 */
export default AdminHomePage
