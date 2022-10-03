import { GetServerSideProps } from 'next'

import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { layoutIds, groupTabIds } from '@constants/routes'
import { routeParams } from '@constants/routes'
import { selectUser, selectParam } from '@selectors/context'
import { withUser } from '@hofs/withUser'
import { withRoutes } from '@hofs/withRoutes'
import { withGroup } from '@hofs/withGroup'
import { withTextContent } from '@hofs/withTextContent'
import { selectPageProps } from '@selectors/context'
import { getGroupFile } from '@services/getGroupFile'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import classNames from 'classnames'
import { Link } from '@components/Link'
import { ActionLink } from '@components/ActionLink'
import { dateTime } from '@helpers/formatters/dateTime'
import { LayoutColumn } from '@components/LayoutColumn'
import { DataGrid } from '@components/DataGrid'
import { RichText } from '@components/RichText'
import { BreadCrumb } from '@components/BreadCrumb'
import { BreadCrumbList } from '@appTypes/routing'
import { FolderContent } from '@appTypes/file'
import { GroupPage } from '@appTypes/page'
import { GroupsPageTextContent } from '@appTypes/content'

declare interface ContentText extends GroupsPageTextContent {
    createdByLabel?: string
}

export interface Props extends GroupPage {
    contentText: ContentText
    fileId: string
    file: FolderContent
}

/**
 * Group file detail template
 */
export const GroupFileDetailPage: (props: Props) => JSX.Element = ({
    fileId,
    file,
    contentText,
    routes,
}) => {
    const { createdByLabel } = contentText ?? {}
    const {
        id,
        path,
        name,
        created,
        createdBy,
        modified,
        modifiedBy,
        text: fileText,
    } = file ?? {}
    const { body } = fileText ?? {}

    const fileDownloadPath: string = `${
        routes.groupFilesRoot
    }/${encodeURIComponent(id)}/download`
    const breadCrumbList: BreadCrumbList = []

    if (path?.length > 0) {
        breadCrumbList.push({
            element: `${routes.groupRoot}`,
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

    const generatedCellClasses = {
        name: classNames({
            ['u-justify-between u-w-full tablet:u-w-1/6']: true,
        }),
        modifiedBy: classNames({
            ['u-justify-between u-w-full tablet:u-w-1/4']: true,
            ['u-hidden tablet:u-block']: !modifiedBy?.text?.userName,
        }),
        lastUpdate: classNames({
            ['u-justify-between u-w-full tablet:u-w-1/6 u-items-center']: true,
        }),
        actions: classNames({
            ['u-w-full tablet:u-w-1/6 tablet:u-justify-end tablet:u-text-right']:
                true,
        }),
    }

    const generatedHeaderCellClasses = {
        name: classNames({
            ['u-text-bold']: true,
        }),
        modifiedBy: classNames({
            ['u-text-bold']: true,
        }),
        lastUpdate: classNames({
            ['u-text-bold']: true,
        }),
    }

    return (
        <>
            <LayoutColumn className="c-page-body">
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
                <h2 className="nhsuk-heading-l">{name}</h2>
                <hr />
                <RichText
                    wrapperElementType="p"
                    bodyHtml={body}
                    className="u-mb-10"
                />
                {createdBy && (
                    <p className="u-mb-14">
                        <span className="u-text-bold u-mr-6">
                            {createdByLabel}
                        </span>
                        <Link
                            href={`${routes.groupMembersRoot}/${createdBy.id}`}
                        >
                            {createdBy.text.userName}
                        </Link>
                    </p>
                )}
                <DataGrid
                    id="group-table-file"
                    text={{
                        caption: 'File data',
                    }}
                    shouldRenderCaption={true}
                    columnList={[
                        {
                            children: 'Name',
                        },
                        {
                            children: 'Modified by',
                        },
                        {
                            children: 'Last update',
                        },
                        {
                            children: 'Actions',
                            className: 'tablet:u-text-right',
                        },
                    ]}
                    rowList={[
                        [
                            {
                                children: name,
                                shouldRenderCellHeader: true,
                                className: generatedCellClasses.name,
                                headerClassName:
                                    generatedHeaderCellClasses.name,
                            },
                            {
                                children: modifiedBy?.text?.userName ?? '',
                                shouldRenderCellHeader: true,
                                className: generatedCellClasses.modifiedBy,
                                headerClassName:
                                    generatedHeaderCellClasses.modifiedBy,
                            },
                            {
                                children: modified
                                    ? dateTime({ value: modified })
                                    : dateTime({ value: created }),
                                shouldRenderCellHeader: true,
                                className: generatedCellClasses.lastUpdate,
                                headerClassName:
                                    generatedHeaderCellClasses.lastUpdate,
                            },
                            {
                                children: (
                                    <ActionLink
                                        href={fileDownloadPath}
                                        text={{
                                            body: 'Download',
                                            ariaLabel: `Download ${name}`,
                                        }}
                                        iconName="icon-download"
                                    />
                                ),
                                shouldRenderCellHeader: false,
                                className: generatedCellClasses.actions,
                            },
                        ],
                    ]}
                    className="u-mb-12"
                />
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
            const user: User = selectUser(context)
            const groupId: string = selectParam(context, routeParams.GROUPID)
            const fileId: string = selectParam(context, routeParams.FILEID)

            props.fileId = fileId
            props.layoutId = layoutIds.GROUP
            props.tabId = groupTabIds.FILES

            /**
             * Get data from services
             */
            try {
                const [groupFile] = await Promise.all([
                    getGroupFile({ user, groupId, fileId }),
                ])

                props.file = groupFile.data
                props.pageTitle = `${props.entityText.title} - ${props.file.name}`
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
export default GroupFileDetailPage
