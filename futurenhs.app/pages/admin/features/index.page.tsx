import { GetServerSideProps } from 'next'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { layoutIds } from '@constants/routes'
import { actions } from '@constants/actions'
import { withUser } from '@helpers/hofs/withUser'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { selectUser, selectPageProps } from '@helpers/selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { LayoutColumnContainer } from '@components/layouts/LayoutColumnContainer'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { Page } from '@appTypes/page'
import { GenericPageTextContent } from '@appTypes/content'
import { FeatureFlag, getFeatureFlags } from '@services/getFeatureFlags'
import { DynamicListContainer } from '@components/layouts/DynamicListContainer'
import { DataGrid } from '@components/layouts/DataGrid'
import Switch from 'react-switch'
import classNames from 'classnames'

declare interface ContentText extends GenericPageTextContent {
    mainHeading: string
}

export interface Props extends Page {
    contentText: ContentText
    featureFlags: Array<FeatureFlag>
}

/**
 * Admin users dashboard template
 */
export const AdminFeaturesPage: (props: Props) => JSX.Element = ({
    contentText,
    featureFlags,
    actions,
    routes,
    user,
    csrfToken,
}) => {
    const { secondaryHeading } = contentText ?? {}
    const handleFeatureToggle = (id, enabled) => {
        return
    }
    const columnList = [
        {
            children: 'Feature',
            className: '',
        },
        {
            children: `Enable`,
            className: 'tablet:u-text-right',
        },
    ]
    const rowList = featureFlags.map(({ id, name, enabled }) => {
        const generatedCellClasses = {
            domain: classNames({
                ['u-justify-between u-w-full tablet:u-w-1/4 o-truncated-text-lines-1']:
                    true,
            }),
        }

        const generatedHeaderCellClasses = {
            domain: classNames({
                ['u-text-bold']: true,
            }),
        }

        const rows = [
            {
                children: <span>{name}</span>,
                className: generatedCellClasses.domain,
                headerClassName: generatedHeaderCellClasses.domain,
            },
            {
                children: (
                    <Switch
                        onChange={() => {
                            handleFeatureToggle(id, enabled)
                        }}
                        checked={enabled}
                    />
                ),
                className: 'u-w-full tablet:u-w-1/8 tablet:u-text-right',
                headerClassName: 'u-hidden',
            },
        ]

        return rows
    })

    return (
        <>
            <LayoutColumnContainer className="u-w-full u-flex u-flex-col tablet:u-flex-row">
                <LayoutColumn tablet={9} className="c-page-body">
                    <h2 className="nhsuk-heading-l">{secondaryHeading}</h2>
                    <DynamicListContainer
                        containerElementType="div"
                        shouldEnableLoadMore={false}
                        className="u-list-none u-p-0"
                    >
                        <DataGrid
                            id="admin-table-domains"
                            columnList={columnList}
                            rowList={rowList}
                            text={{
                                caption: 'Allowed domains',
                            }}
                        />
                    </DynamicListContainer>
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
            routeId: 'd7016549-cb04-457e-b5d8-caa49e408ecb',
        },
        [withUser, withRoutes, withTextContent],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get data from request context
             */
            const props: Partial<Props> = selectPageProps(context)
            const user: User = selectUser(context)

            props.layoutId = layoutIds.ADMIN

            if (!props.actions.includes(actions.SITE_ADMIN_VIEW)) {
                return {
                    notFound: true,
                }
            }

            /**
             * Get data from services
             */
            try {
                const { data } = await getFeatureFlags({ user })
                props.featureFlags = data
            } catch (error) {
                return handleSSRErrorProps({ props, error })
            }

            /**
             * Return data to page template
             */
            return handleSSRSuccessProps({ props, context })
        }
    )

/**
 * Export page template
 */
export default AdminFeaturesPage
