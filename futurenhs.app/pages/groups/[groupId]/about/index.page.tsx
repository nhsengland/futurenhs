import { GetServerSideProps } from 'next'
import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { layoutIds, groupTabIds } from '@constants/routes'
import { withUser } from '@helpers/hofs/withUser'
import { withGroup } from '@helpers/hofs/withGroup'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { selectPageProps } from '@helpers/selectors/context'
import { GetServerSidePropsContext } from '@appTypes/next'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { GroupPage } from '@appTypes/page'

export interface Props extends GroupPage {}

export const GroupAboutUsPage: (props: Props) => JSX.Element = ({
    user,
    actions,
    contentText,
}) => {
    const { secondaryHeading } = contentText

    return (
        <>
            <LayoutColumn tablet={12} className="c-page-body">
                <h2>{secondaryHeading}</h2>
            </LayoutColumn>
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
            routeId: 'cfe2a86c-f17f-4fbe-843f-7f43f5d7ad06',
        },
        [withUser, withRoutes, withGroup, withTextContent],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get data from request context
             */
            const props: Partial<Props> = selectPageProps(context)
            props.layoutId = layoutIds.GROUP
            props.tabId = groupTabIds.ABOUT
            props.pageTitle = `${props.entityText.title} - ${props.contentText.subTitle}`

            /**
             * Return data to page template
             */
            return handleSSRSuccessProps({ props, context })
        }
    )

/**
 * Export page template
 */
export default GroupAboutUsPage
