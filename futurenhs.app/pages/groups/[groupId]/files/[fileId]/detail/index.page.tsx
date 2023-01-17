import { GetServerSideProps } from 'next'

import { pipeSSRProps } from '@helpers/util/ssr/pipeSSRProps'
import { handleSSRSuccessProps } from '@helpers/util/ssr/handleSSRSuccessProps'
import { handleSSRErrorProps } from '@helpers/util/ssr/handleSSRErrorProps'
import { layoutIds, groupTabIds } from '@constants/routes'
import { routeParams } from '@constants/routes'
import { selectUser, selectParam } from '@helpers/selectors/context'
import { withUser } from '@helpers/hofs/withUser'
import { withRoutes } from '@helpers/hofs/withRoutes'
import { withGroup } from '@helpers/hofs/withGroup'
import { withTextContent } from '@helpers/hofs/withTextContent'
import { selectPageProps } from '@helpers/selectors/context'
import { getGroupFile } from '@services/getGroupFile'
import { GetServerSidePropsContext } from '@appTypes/next'
import { User } from '@appTypes/user'
import classNames from 'classnames'
import { Link } from '@components/generic/Link'
import { ActionLink } from '@components/generic/ActionLink'
import { dateTime, dateTimeWithTime } from '@helpers/formatters/dateTime'
import { LayoutColumn } from '@components/layouts/LayoutColumn'
import { DataGrid } from '@components/layouts/DataGrid'
import { RichText } from '@components/generic/RichText'
import { BreadCrumb } from '@components/generic/BreadCrumb'
import { BreadCrumbList } from '@appTypes/routing'
import { FolderContent } from '@appTypes/file'
import { FileVersion } from '@appTypes/file'
import { GroupPage } from '@appTypes/page'
import { GroupsPageTextContent } from '@appTypes/content'
import { version } from 'react'

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
        versions,
        text: fileText,
    } = file ?? {}

const readableFileSize = (bytes, si=true, dp=2) => {
    const thresh = si ? 1000 : 1024;
  
    if (Math.abs(bytes) < thresh) {
      return bytes + ' B';
    }
  
    const units = ['kB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];
    let u = -1;
    const r = 10**dp;
  
    do {
      bytes /= thresh;
      ++u;
    } while (Math.round(Math.abs(bytes) * r) / r >= thresh && u < units.length - 1);
  
  
    return bytes.toFixed(dp) + ' ' + units[u];
  }

    const { body } = fileText ?? {}
   
    const fileDownloadPath: string = `${
        routes.groupFilesRoot
    }/${encodeURIComponent(id)}/download`;

    const fileViewPath: string = `${
        routes.groupFilesRoot
    }/${encodeURIComponent(id)}`
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
            ['u-hidden tablet:u-block']: modifiedBy?.text?.userName,
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

    let versionRows = []
    for (let i = 0; i < versions?.length; i++) {
            const versionPath: string = `${
                routes.groupFilesRoot
                }/${encodeURIComponent(versions[i].id)}`
            versionRows.push([
            {
                children: readableFileSize(versions[i].size),
                // children: sizeInKb,
                shouldRenderCellHeader: true,
                className: generatedCellClasses.name,
                headerClassName: generatedHeaderCellClasses.name,
            },
            {
                children: versions[i].lastUpdated.text.userName ?? '',
                shouldRenderCellHeader: true,
                className: generatedCellClasses.modifiedBy,
                headerClassName: generatedHeaderCellClasses.modifiedBy,
            },
            {
                children: versions[i].modifiedAt
                    ? dateTimeWithTime({ value: versions[i].modifiedAt })
                    : dateTimeWithTime({ value: created }),
                shouldRenderCellHeader: true,
                className: generatedCellClasses.lastUpdate,
                headerClassName: generatedHeaderCellClasses.lastUpdate,
            },
            {
                children: (
                    <ActionLink
                        href={versionPath}
                        text={{
                            body: 'View',
                            ariaLabel: `View ${name}`,
                        }}
                        iconName="icon-view"
                    />
                ),
                shouldRenderCellHeader: false,
                className: generatedCellClasses.actions,
            },
        ])
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
                                children: modifiedBy.text.userName ?? '',
                                shouldRenderCellHeader: true,
                                className: generatedCellClasses.modifiedBy,
                                headerClassName:
                                    generatedHeaderCellClasses.modifiedBy,
                            },
                            {
                                children: modified
                                    ? dateTimeWithTime({ value: modified })
                                    : dateTimeWithTime({ value: created }),
                                shouldRenderCellHeader: true,
                                className: generatedCellClasses.lastUpdate,
                                headerClassName:
                                    generatedHeaderCellClasses.lastUpdate,
                            },
                            {
                                children: (
                                    <p>
                                    <ActionLink
                                        href={fileViewPath}
                                        text={{
                                            body: 'View',
                                            ariaLabel: `View ${name}`,
                                        }}
                                        iconName="icon-view"
                                    />
                                    <ActionLink
                                        href={fileDownloadPath}
                                        text={{
                                            body: 'Download',
                                            ariaLabel: `Download ${name}/download`,
                                        }}
                                        iconName="icon-download"
                                    />
                                    </p>
                                ),
                                shouldRenderCellHeader: false,
                                className: generatedCellClasses.actions,
                            },
                        ],
                    ]}
                    className="u-mb-12"
                />

                {versions ? (
                    <DataGrid
                        id="group-table-file"
                        text={{
                            caption: 'Version History',
                        }}
                        shouldRenderCaption={true}
                        columnList={[
                            {
                                children: 'Size',
                            },
                            {
                                children: 'Modified By',
                            },
                            {
                                children: 'Modified At',
                            },
                            {
                                children: 'Actions',
                                className: 'tablet:u-text-right',
                            },
                        ]}
                        rowList={versionRows}
                        className="u-mb-12"
                    />
                ) : (
                    <h3>There are no previous versions of this file</h3>
                )}
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
