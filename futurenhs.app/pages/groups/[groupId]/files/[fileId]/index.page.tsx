import { GetServerSideProps } from 'next'

import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { layoutIds, groupTabIds } from '@constants/routes'
import { routeParams } from '@constants/routes'
import {
    selectUser,
    selectParam,
    selectCsrfToken,
    selectPageProps,
} from '@helpers/selectors/context'
import { withUser } from '@helpers/hofs/withUser'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { withGroup } from '@helpers/hofs/withGroup'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { getGroupFile } from '@services/getGroupFile'
import { getGroupFileView } from '@services/getGroupFileView'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import { Link } from '@components/generic/Link'
import { AriaLiveRegion } from '@components/blocks/AriaLiveRegion'
import { CollaboraFilePreview } from '@components/blocks/CollaboraFilePreview'
import { WarningCallout } from '@components/generic/WarningCallout'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { BreadCrumb } from '@components/generic/BreadCrumb'
import { SVGIcon } from '@components/generic/SVGIcon'
import { BreadCrumbList } from '@appTypes/routing'

import { FolderContent } from '@appTypes/file'
import { GroupPage } from '@appTypes/page'
import { GroupsPageTextContent } from '@appTypes/content'
import { CollaboraConnectionParams } from '@appTypes/collabora'

declare interface ContentText extends GroupsPageTextContent {
    previewLabel?: string
    createdByLabel?: string
}

export interface Props extends GroupPage {
    contentText: ContentText
    fileId: string
    file: FolderContent
    preview: CollaboraConnectionParams
    shouldRenderGroupHeader?: boolean
    shouldRenderPhaseBanner?: boolean
    shouldRenderBreadCrumb?: boolean
}

/**
 * Group file preview template
 */
export const GroupFilePreviewPage: (props: Props) => JSX.Element = ({
    csrfToken,
    fileId,
    file,
    preview,
    routes,
}) => {
    const { path, name } = file ?? {}
    const { accessToken, wopiClientUrl } = preview ?? {}

    const breadCrumbList: BreadCrumbList = []
    const hasCollaboraData: boolean =
        Boolean(accessToken) && Boolean(wopiClientUrl)
    const fileDetailPath: string = `${routes.groupFilesRoot}/${fileId}/detail`

    if (path?.length > 0) {
        breadCrumbList.push({
            element: `${routes.groupFoldersRoot}`,
            text: 'Files',
        })

        path?.forEach(({ element, text }) => {
            if (element !== fileId) {
                breadCrumbList.push({
                    element: `${routes.groupFoldersRoot}/${element}`,
                    text: text,
                })
            }
        })
    }

    const hasBreadCrumb: boolean = breadCrumbList.length > 0

    return (
        <>
            <LayoutColumn className="c-page-body u-pt-4">
                {hasBreadCrumb && (
                    <BreadCrumb
                        text={{
                            ariaLabel: 'Folders',
                        }}
                        breadCrumbList={breadCrumbList}
                        shouldLinkCrumbs={false}
                        className="u-text-lead u-mb-10 u-fill-theme-0"
                    />
                )}
                <h1 className="nhsuk-heading-l">{name}</h1>
                <hr />
                <noscript>
                    <WarningCallout
                        headingLevel={3}
                        text={{
                            heading: 'Important',
                            body: 'JavaScript must be enabled in your browser to view this file',
                        }}
                    />
                </noscript>
                <AriaLiveRegion>
                    {hasCollaboraData && (
                        <CollaboraFilePreview
                            csrfToken={csrfToken}
                            accessToken={accessToken}
                            wopiClientUrl={wopiClientUrl}
                        />
                    )}
                </AriaLiveRegion>
                <p>
                    <Link href={fileDetailPath} aria-label={`File information of ${name}`}>
                        <a>
                            <SVGIcon
                                name="icon-info"
                                className="u-w-6 u-h-10 u-mr-1 u-align-middle"
                            />
                            File information
                        </a>
                    </Link>
                </p>
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
            routeId: 'b74b9b6b-0462-4c2a-8859-51d0df17f68f',
        },
        [withUser, withRoutes, withGroup, withTextContent],
        async (context: GetServerSidePropsContext) => {
            /**
             * Get data from request context
             */
            const props: Partial<any> = selectPageProps(context)
            const csrfToken: string = selectCsrfToken(context)
            const user: User = selectUser(context)
            const groupId: string = selectParam(context, routeParams.GROUPID)
            const fileId: string = selectParam(context, routeParams.FILEID)

            props.fileId = fileId
            props.layoutId = layoutIds.GROUP
            props.tabId = groupTabIds.FILES
            props.shouldRenderPhaseBanner = false
            props.shouldRenderBreadCrumb = false
            props.shouldRenderGroupHeader = false

            /**
             * Get data from services
             */
            try {
                const [groupFile, groupFileView] = await Promise.all([
                    getGroupFile({ user, groupId, fileId }),
                    getGroupFileView({
                        user,
                        groupId,
                        fileId,
                        cookies: context.req?.cookies,
                    }),
                ])

                props.csrfToken = csrfToken
                props.file = groupFile.data
                props.preview = groupFileView.data
                props.pageTitle = `${props.entityText.title} - ${props.file.name}`
            } catch (error) {
                console.log(error)

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
export default GroupFilePreviewPage
