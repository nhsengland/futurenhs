import { useMemo, useState } from 'react'
import Link from 'next/link'
import { useRouter } from 'next/router'
import classNames from 'classnames'

import { actions as userActions } from '@constants/actions'
import { iconMap } from '@constants/icons'
import { Dialog } from '@components/Dialog'
import { BreadCrumb } from '@components/BreadCrumb'
import { SVGIcon } from '@components/SVGIcon'
import { dateTime } from '@helpers/formatters/dateTime'
import { LayoutColumnContainer } from '@components/LayoutColumnContainer'
import { LayoutColumn } from '@components/LayoutColumn'
import { DataGrid } from '@components/DataGrid'
import { RichText } from '@components/RichText'
import { ActionLink } from '@components/ActionLink'
import { PaginationWithStatus } from '@components/PaginationWithStatus'
import { getGroupFolderContents } from '@services/getGroupFolderContents'
import { BreadCrumbList } from '@appTypes/routing'
import { DataRow } from '@components/DataGrid/interfaces'

import { Props } from './interfaces'

/**
 * Group folder contents template
 */
export const GroupFolderContentsTemplate: (props: Props) => JSX.Element = ({
    user,
    groupId,
    contentText,
    folderId,
    folder,
    folderContents,
    actions,
    routes,
    pagination,
}) => {
    const router = useRouter()

    const [isCancelDeleteModalOpen, setIsCancelDeleteModalOpen] =
        useState(false)
    const [folderContentsList, setFolderContentsList] = useState(folderContents)
    const [dynamicPagination, setPagination] = useState(pagination)

    const { id, text: folderText, path } = folder ?? {}
    const { name, body } = folderText ?? {}
    const {
        foldersHeading,
        noFolders,
        createFolder,
        updateFolder,
        deleteFolder,
        createFile,
    } = contentText ?? {}

    const hasFolderContents: boolean = folderContentsList?.length > 0
    const hasAddFileAction: boolean = actions?.includes(
        userActions.GROUPS_FILES_ADD
    )
    const hasAddFolderAction: boolean = actions?.includes(
        userActions.GROUPS_FOLDERS_ADD
    )
    const hasEditFolderAction: boolean = actions?.includes(
        userActions.GROUPS_FOLDERS_EDIT
    )
    const hasDeleteFolderAction: boolean = false // Temporarily removing for private beta //actions?.includes(userActions.GROUPS_FOLDERS_DELETE);

    const folderUpdatePath: string = `${routes.groupFoldersRoot}/${folderId}/update`
    const folderDeletePath: string = `${routes.groupFoldersRoot}/${folderId}/delete`

    const breadCrumbList: BreadCrumbList = []

    if (path?.length > 0) {
        breadCrumbList.push({
            element: routes.groupFoldersRoot,
            text: 'Files',
        })
    }

    path?.forEach(({ element, text }) => {
        if (element !== id) {
            breadCrumbList.push({
                element: `${routes.groupFoldersRoot}/${element}`,
                text: text,
            })
        }
    })

    const hasBreadCrumb: boolean = breadCrumbList.length > 0

    const columnList = useMemo((): DataRow => {
        const columns: DataRow = [
            {
                children: 'Type',
                className: 'u-text-center tablet:u-w-24 desktop:u-w-1/12',
            },
            {
                children: 'Name',
                className: folderId ? 'tablet:u-w-1/5' : 'tablet:u-w-4/12',
            },
            {
                children: 'Description',
                className: folderId
                    ? 'tablet:u-w-1/5 desktop:u-w-1/4'
                    : 'tablet:u-w-4/12 desktop:u-w-1/4',
            },
            {
                children: 'Modified',
            },
        ]

        if (folderId) {
            columns.push({
                children: 'Actions',
                className: 'tablet:u-text-right'
            })
        }

        return columns
    }, [folderId])

    const gridRowList = useMemo((): Array<DataRow> => {
        const rows: Array<DataRow> = []

        folderContentsList?.forEach(
            ({
                id,
                type,
                extension,
                name,
                text,
                modified,
                modifiedBy,
                createdBy,
            }) => {
                const row: DataRow = []

                const { body } = text ?? {}

                const isFolder: boolean = type === 'folder'
                const itemPath: string = `${
                    isFolder ? routes.groupFoldersRoot : routes.groupFilesRoot
                }/${encodeURIComponent(id)}`
                const iconLabel: string = extension || 'Folder'
                const fileDetailPath: string = `${
                    routes.groupFilesRoot
                }/${encodeURIComponent(id)}/detail`
                const fileDownloadPath: string = `${
                    routes.groupFilesRoot
                }/${encodeURIComponent(id)}/download`
                const iconName: string = isFolder
                    ? 'icon-folder'
                    : iconMap[extension]

                const generatedCellClasses = {
                    type: classNames({
                        ['u-text-center u-text-base u-items-center u-justify-end']:
                            true,
                    }),
                    name: classNames({
                        ['u-flex-grow u-w-4/6 u-truncate']: true,
                    }),
                    description: classNames({
                        ['u-flex-col u-w-full']: true,
                        ['u-hidden']: !body,
                    }),
                    modified: classNames({
                        ['u-hidden tablet:u-flex-col u-w-full']: true,
                        ['u-hidden']: isFolder,
                    }),
                    actions: classNames({
                        ['u-w-full u-justify-between tablet:u-text-right']: true,
                        ['u-hidden']: isFolder,
                    }),
                }

                const generatedHeaderCellClasses = {
                    description: classNames({
                        ['u-text-bold']: true,
                    }),
                    modified: classNames({
                        ['u-text-bold']: true,
                    }),
                    actions: classNames({
                        ['u-w-full u-hidden']: true,
                        ['u-justify-between tablet:u-block']: !isFolder,
                    }),
                }

                row.push({
                    children: (
                        <>
                            <SVGIcon name={iconName} className="u-w-4 u-h-6" />
                            <span className="u-block u-text-bold u-sr-only tablet:u-not-sr-only">
                                {iconLabel}
                            </span>
                        </>
                    ),
                    shouldRenderCellHeader: false,
                    className: generatedCellClasses.type,
                })
                row.push({
                    children: 
                        <ActionLink 
                            href={itemPath}
                            text={{
                                body: name,
                                ariaLabel: `View ${name}`
                            }} />,
                    shouldRenderCellHeader: false,
                    className: generatedCellClasses.name,
                })
                row.push({
                    children: (
                        <RichText
                            bodyHtml={body}
                            wrapperElementType="span"
                            className="o-truncated-text-lines-3"
                        />
                    ),
                    shouldRenderCellHeader: true,
                    className: generatedCellClasses.description,
                    headerClassName: generatedHeaderCellClasses.description,
                }),
                    row.push({
                        children: isFolder ? (
                            ''
                        ) : (
                            <RichText
                                bodyHtml={`<p class='u-mb-1'>${dateTime({
                                    value: modified,
                                })}</p>${
                                    modifiedBy?.text?.userName &&
                                    '<p class="u-mb-1"><span class="u-text-bold">By</span> ' +
                                        modifiedBy.text.userName +
                                        '</p>'
                                }${
                                    createdBy?.text?.userName &&
                                    '<p><span class="u-text-bold">Author</span> ' +
                                        createdBy.text.userName +
                                        '</p>'
                                }`}
                            />
                        ),
                        shouldRenderCellHeader: true,
                        className: generatedCellClasses.modified,
                        headerClassName: generatedHeaderCellClasses.modified,
                    })

                if (folderId) {
                    row.push({
                        children: isFolder ? (
                            ''
                        ) : (
                            <>
                                {fileDownloadPath &&
                                    <ActionLink 
                                        href={fileDownloadPath}
                                        text={{
                                            body: 'Download file',
                                            ariaLabel: `Download ${name}`
                                        }}
                                        iconName="icon-download"
                                        className="u-block tablet:u-mb-4 u-align-top" />
                                }
                                {fileDetailPath && 
                                    <ActionLink 
                                        href={fileDetailPath}
                                        text={{
                                            body: 'View details',
                                            ariaLabel: `View details of ${name}`
                                        }}
                                        iconName="icon-view"/>
                                }
                            </>
                        ),
                        shouldRenderCellHeader: true,
                        className: generatedCellClasses.actions,
                        headerClassName: generatedHeaderCellClasses.actions,
                    })
                }

                rows.push(row)
            }
        )

        return rows
    }, [folderContentsList, folderId])

    const handleGetPage = async ({
        pageNumber: requestedPageNumber,
        pageSize: requestedPageSize,
    }) => {
        const { data: additionalFiles, pagination } =
            await getGroupFolderContents({
                user: user,
                groupId: groupId,
                folderId: folderId,
                pagination: {
                    pageNumber: requestedPageNumber,
                    pageSize: requestedPageSize,
                },
            })

        setFolderContentsList([...folderContentsList, ...additionalFiles])
        setPagination(pagination)
    }

    const handleDeleteFolder = (event: any): void => {
        event.preventDefault()

        setIsCancelDeleteModalOpen(true)
    }
    const handleDeleteFolderCancel = () => {
        setIsCancelDeleteModalOpen(false)
    }
    const handleDeleteFolderConfirm = () => {
        setIsCancelDeleteModalOpen(false)
        router.push(folderDeletePath)
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
                        truncationMinPathLength={5}
                        truncationStartIndex={1}
                        truncationEndIndex={breadCrumbList.length - 3}
                        className="u-text-lead u-mb-10 u-fill-theme-0"
                    />
                )}
                {folderId && (
                    <>
                        <LayoutColumnContainer>
                            <LayoutColumn
                                tablet={6}
                                desktop={8}
                                className="u-self-center"
                            >
                                <h2 className="nhsuk-heading-l o-truncated-text-lines-3">
                                    {name}
                                </h2>
                            </LayoutColumn>
                            {folderId &&
                                (hasEditFolderAction ||
                                    hasDeleteFolderAction) && (
                                    <LayoutColumn
                                        tablet={6}
                                        desktop={4}
                                        className="tablet:u-text-right"
                                    >
                                        <p className="u-mb-0">
                                            {hasDeleteFolderAction && (
                                                <>
                                                    <Link
                                                        href={folderDeletePath}
                                                    >
                                                        <a
                                                            className="c-button c-button--outline u-mr-2 u-my-4 u-w-full tablet:u-w-auto tablet:u-my-0 u-drop-shadow"
                                                            onClick={
                                                                handleDeleteFolder
                                                            }
                                                        >
                                                            {deleteFolder}
                                                        </a>
                                                    </Link>
                                                    <Dialog
                                                        id="dialog-delete-folder"
                                                        isOpen={
                                                            isCancelDeleteModalOpen
                                                        }
                                                        text={{
                                                            cancelButton:
                                                                'Cancel',
                                                            confirmButton:
                                                                'Yes, discard',
                                                        }}
                                                        cancelAction={
                                                            handleDeleteFolderCancel
                                                        }
                                                        confirmAction={
                                                            handleDeleteFolderConfirm
                                                        }
                                                    >
                                                        <h3>
                                                            Folder will be
                                                            deleted
                                                        </h3>
                                                        <p className="u-text-bold">
                                                            Any folder contents
                                                            will also be
                                                            discarded. Are you
                                                            sure you wish to
                                                            proceed?
                                                        </p>
                                                    </Dialog>
                                                </>
                                            )}
                                            {hasEditFolderAction && (
                                                <Link href={folderUpdatePath}>
                                                    <a className="c-button c-button--outline u-w-full tablet:u-w-auto u-drop-shadow">
                                                        {updateFolder}
                                                    </a>
                                                </Link>
                                            )}
                                        </p>
                                    </LayoutColumn>
                                )}
                        </LayoutColumnContainer>
                        <hr />
                    </>
                )}
                {!folderId && (
                    <h2 className="nhsuk-heading-l">{foldersHeading}</h2>
                )}
                {body && (
                    <RichText
                        wrapperElementType="p"
                        bodyHtml={body}
                        className="u-mb-10"
                    />
                )}
                {!hasFolderContents && !folderId && (
                    <p className="u-mb-12">{noFolders}</p>
                )}
                {(hasAddFolderAction || hasAddFileAction) && (
                    <p className="u-mb-6">
                        {hasAddFolderAction && (
                            <Link
                                href={`${routes.groupFoldersRoot}/create${
                                    folderId ? '?folderId=' + folderId : ''
                                }`}
                            >
                                <a className="c-button c-button--outline u-mr-2 u-mb-4 u-w-full tablet:u-w-72 u-drop-shadow">
                                    {createFolder}
                                </a>
                            </Link>
                        )}
                        {folderId && hasAddFileAction && (
                            <Link
                                href={{
                                    pathname: `${routes.groupFilesRoot}/create`,
                                    query: {
                                        folderId: folderId,
                                    },
                                }}
                            >
                                <a className="c-button c-button--outline u-w-full tablet:u-w-72 u-drop-shadow">
                                    {createFile}
                                </a>
                            </Link>
                        )}
                    </p>
                )}
                {hasFolderContents && (
                    <>
                        <DataGrid
                            id="group-table-files"
                            text={{
                                caption: 'Group folders',
                            }}
                            columnList={columnList}
                            rowList={gridRowList}
                        />
                        <PaginationWithStatus
                            id="file-list-pagination"
                            shouldEnableLoadMore={true}
                            getPageAction={handleGetPage}
                            {...dynamicPagination}
                        />
                    </>
                )}
            </LayoutColumn>
        </>
    )
}
