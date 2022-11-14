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
import { useEffect, useState } from 'react'

export interface Props extends GroupPage {}

export const GroupWhiteboardPage: (props: Props) => JSX.Element = ({
    user,
    actions,
    contentText,
}) => {
    const { secondaryHeading } = contentText
    const [Comp, setComp] = useState(null)
    useEffect(() => {
        window['EXCALIDRAW_ASSET_PATH'] = '/js/excalidraw/dist/'
        import('@excalidraw/excalidraw').then((comp) =>
            setComp(comp.Excalidraw)
        )
    }, [])
    return (
        <>
            <LayoutColumn className="c-page-body u-pt-4">
                <h2>{secondaryHeading}</h2>
                <noscript>
                    You need to enable JavaScript to run this app.
                </noscript>
                <div style={{ width: '100%', height: '800px' }}>
                    {Comp && <Comp />}
                </div>
            </LayoutColumn>
        </>
    )
}

/**
 * Get props to inject into page on the initial server-side request
 */
/**
 * Get props to inject into page on the initial server-side request
 */
export const getServerSideProps: GetServerSideProps = async (
    context: GetServerSidePropsContext
) =>
    await pipeSSRProps(
        context,
        {
            routeId: 'b5df79f1-e980-4339-9035-ad376322e8a9',
        },
        [withUser, withRoutes, withGroup, withTextContent],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get data from request context
             */
            const props: Partial<Props> = selectPageProps(context)
            props.layoutId = layoutIds.GROUP
            props.tabId = groupTabIds.WHITEBOARD
            props.pageTitle = `${props.entityText.title} - ${props.contentText.subTitle}`
            /**
             * Return data to page template
             */
            return handleSSRSuccessProps({ props, context })
        }
    )

export default GroupWhiteboardPage
