import { useMemo, useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/router';
import classNames from 'classnames';

import { actions as userActions } from '@constants/actions';
import { routeParams } from '@constants/routes';
import { iconMap } from '@constants/icons';
import { Dialog } from '@components/Dialog';
import { BreadCrumb } from '@components/BreadCrumb';
import { SVGIcon } from '@components/SVGIcon';
import { dateTime } from '@helpers/formatters/dateTime';
import { AriaLiveRegion } from '@components/AriaLiveRegion';
import { LayoutColumnContainer } from '@components/LayoutColumnContainer';
import { LayoutColumn } from '@components/LayoutColumn';
import { DataGrid } from '@components/DataGrid';
import { RichText } from '@components/RichText';
import { PaginationWithStatus } from '@components/PaginationWithStatus';
import { getGroupFolderContents } from '@services/getGroupFolderContents';
import { getRouteToParam } from '@helpers/routing/getRouteToParam';
import { BreadCrumbList } from '@appTypes/routing';

import { Props } from './interfaces';

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
    pagination
}) => {

    const router = useRouter();

    const [isCancelDeleteModalOpen, setIsCancelDeleteModalOpen] = useState(false);
    const [folderContentsList, setFolderContentsList] = useState(folderContents);
    const [dynamicPagination, setPagination] = useState(pagination);

    const { id,
            text: folderText,
            path } = folder ?? {};
    const { name, body } = folderText ?? {};
    const { foldersHeading,
            noFolders,
            createFolder,
            updateFolder,
            deleteFolder,
            createFile } = contentText ?? {};

    const hasFolderContents: boolean = folderContentsList?.length > 0;
    const hasAddFileAction: boolean = actions?.includes(userActions.GROUPS_FILES_ADD);
    const hasAddFolderAction: boolean = actions?.includes(userActions.GROUPS_FOLDERS_ADD);
    const hasEditFolderAction: boolean = actions?.includes(userActions.GROUPS_FOLDERS_EDIT);
    const hasDeleteFolderAction: boolean = actions?.includes(userActions.GROUPS_FOLDERS_DELETE);

    const groupBasePath: string = getRouteToParam({
        router: router,
        paramName: routeParams.GROUPID,
        shouldIncludeParam: true
    });

    const folderBasePath: string = folderId ? getRouteToParam({
        router: router,
        paramName: routeParams.FOLDERID
    }) : `${groupBasePath}/folders`;

    const fileBasePath: string = `${groupBasePath}/files`;
    const folderUpdatePath: string = `${groupBasePath}/folders/${folderId}/update`;
    const folderDeletePath: string = `${groupBasePath}/folders/${folderId}/delete`;

    const breadCrumbList: BreadCrumbList = [];

    if (path?.length > 0) {

        breadCrumbList.push({
            element: folderBasePath,
            text: 'Files'
        });

    }

    path?.forEach(({ element, text }) => {

        if (element !== id) {

            breadCrumbList.push({
                element: `${groupBasePath}/folders/${element}`,
                text: text
            });

        }

    });

    const hasBreadCrumb: boolean = breadCrumbList.length > 0;

    const gridRowList = useMemo(() => folderContentsList?.map(({
        id,
        type,
        extension,
        name,
        text,
        modified,
        modifiedBy,
        createdBy,
        downloadLink }) => {

            const { body } = text ?? {};

            const isFolder: boolean = type === 'folder';
            const itemPath: string = `${isFolder ? folderBasePath : fileBasePath}/${encodeURIComponent(id)}`;
            const fileDetailPath: string = `${fileBasePath}/${encodeURIComponent(id)}/detail`;
            const fileDownloadPath: string = downloadLink;

            let iconName: string = isFolder ? 'icon-folder' : iconMap[extension];

            const generatedCellClasses = {
                type: classNames({
                    ['u-text-center u-text-base u-w-1/6 tablet:u-w-8 u-items-center u-justify-end']: true
                }),
                name: classNames({
                    ['u-w-5/6 tablet:u-w-1/6']: true
                }),
                description: classNames({
                    ['u-flex-col u-w-full tablet:u-w-1/3']: true,
                    ['u-hidden']: !body
                }),
                modified: classNames({
                    ['u-hidden tablet:u-flex-col u-w-full tablet:u-w-1/4']: true,
                    ['u-hidden']: isFolder
                }),
                actions: classNames({
                    ['u-w-full tablet:u-w-1/6 u-justify-between']: true,
                    ['u-hidden']: isFolder
                })
            };

            const generatedHeaderCellClasses = {
                type: classNames({
                    ['u-hidden']: true
                }),
                name: classNames({
                    ['u-hidden']: true
                }),
                description: classNames({
                    ['u-text-bold']: true
                }),
                modified: classNames({
                    ['u-text-bold']: true
                }),
                actions: classNames({
                    ['u-hidden']: true
                })
            };

            return [
                {
                    children: <><SVGIcon name={iconName} className="u-w-4 u-h-6" /><span className="u-text-bold u-hidden tablet:u-block">{extension}</span></>,
                    className: generatedCellClasses.type,
                    headerClassName: generatedHeaderCellClasses.type
                },
                {
                    children: <Link href={itemPath}><a className="o-truncated-text-lines-3">{name}</a></Link>,
                    className: generatedCellClasses.name,
                    headerClassName: generatedHeaderCellClasses.name
                },
                {
                    children: <RichText bodyHtml={body} wrapperElementType='span' className='o-truncated-text-lines-3' />,
                    className: generatedCellClasses.description,
                    headerClassName: generatedHeaderCellClasses.description
                },
                {
                    children: isFolder ? '' : <RichText bodyHtml={`<p class='u-mb-1'>${dateTime({ value: modified })}</p>${modifiedBy?.text?.userName && '<p class="u-mb-1"><span class="u-text-bold">By</span> ' + modifiedBy.text.userName + '</p>'}${createdBy?.text?.userName && '<p><span class="u-text-bold">Author</span> ' + createdBy.text.userName + '</p>'}`} />,
                    className: generatedCellClasses.modified,
                    headerClassName: generatedHeaderCellClasses.modified
                },
                {
                    children: isFolder ? '' : <>
                        {fileDownloadPath && 
                            <Link href={fileDownloadPath}><a className="u-block u-mb-4 u-align-top"><SVGIcon name="icon-download" className="u-w-4 u-h-6 u-mr-2 u-align-middle u-fill-theme-8" />Download file</a></Link>
                        }
                        {fileDetailPath &&
                            <Link href={fileDetailPath}><a className="u-block u-align-top"><SVGIcon name="icon-view" className="u-w-4 u-h-6 u-mr-2 u-align-middle u-fill-theme-8" />View details</a></Link>
                        }
                    </>,
                    className: generatedCellClasses.actions,
                    headerClassName: generatedHeaderCellClasses.actions
                }
            ]

    }), [folderContentsList, folderId]);

    const handleGetPage = async ({
        pageNumber: requestedPageNumber,
        pageSize: requestedPageSize
    }) => {

        const { data: additionalFiles, pagination } = await getGroupFolderContents({
            user: user,
            groupId: groupId,
            folderId: folderId,
            pagination: {
                pageNumber: requestedPageNumber,
                pageSize: requestedPageSize
            }
        });

        setFolderContentsList([...folderContentsList, ...additionalFiles]);
        setPagination(pagination);

    };

    const handleDeleteFolder = (event: any): void => {
        
        event.preventDefault();

        setIsCancelDeleteModalOpen(true);

    };
    const handleDeleteFolderCancel = () => {

        setIsCancelDeleteModalOpen(false);

    };
    const handleDeleteFolderConfirm = () => {

        setIsCancelDeleteModalOpen(false);
        router.push(folderDeletePath);

    };

    return (

        <>
            <LayoutColumn className="c-page-body">
                {hasBreadCrumb &&
                    <BreadCrumb
                        text={{
                            ariaLabel: 'Folders'
                        }}
                        breadCrumbList={breadCrumbList}
                        shouldLinkCrumbs={false}
                        className="u-text-lead u-mb-10 u-fill-theme-0" />
                }
                {folderId &&
                    <>
                        <LayoutColumnContainer>
                            <LayoutColumn tablet={6} desktop={8} className="u-self-center">
                                <h2 className="nhsuk-heading-l u-m-0 o-truncated-text-lines-3">{name}</h2>
                            </LayoutColumn>
                            {(folderId && (hasEditFolderAction || hasDeleteFolderAction)) &&
                                <LayoutColumn tablet={6} desktop={4} className="tablet:u-text-right">
                                    <p className="u-mb-0">
                                        <Link href={folderDeletePath}>
                                            <a className="c-button c-button--outline u-mr-2 u-my-4 u-w-full tablet:u-w-auto tablet:u-my-0 u-drop-shadow" onClick={handleDeleteFolder}>
                                                {deleteFolder}
                                            </a>
                                        </Link>
                                        <Dialog
                                            id="dialog-delete-folder"
                                            isOpen={isCancelDeleteModalOpen}
                                            text={{
                                                cancelButton: 'Cancel',
                                                confirmButton: 'Yes, discard'
                                            }}
                                            cancelAction={handleDeleteFolderCancel}
                                            confirmAction={handleDeleteFolderConfirm}>
                                                <h3>Folder will be deleted</h3>
                                                <p className="u-text-bold">Any folder contents will also be discarded. Are you sure you wish to proceed?</p>
                                        </Dialog>
                                        <Link href={folderUpdatePath}>
                                            <a className="c-button c-button--outline u-w-full tablet:u-w-auto u-drop-shadow">
                                                {updateFolder}
                                            </a>
                                        </Link>
                                    </p>
                                </LayoutColumn>
                            }
                        </LayoutColumnContainer>
                        <hr />
                    </>
                }
                {!folderId &&
                    <h2 className="nhsuk-heading-l">{foldersHeading}</h2>
                }
                {body &&
                    <RichText 
                        wrapperElementType="p" 
                        bodyHtml={body}
                        className="u-mb-10" />
                }
                {(!hasFolderContents && !folderId) &&
                    <p className="u-mb-12">{noFolders}</p>
                }
                {(hasAddFolderAction || hasAddFileAction) &&
                    <p className="u-mb-6">
                        {hasAddFolderAction &&
                            <Link href={`${groupBasePath}/folders/create${folderId ? '?folderId=' + folderId : ''}`}>
                                <a className="c-button c-button--outline u-mr-2 u-mb-4 u-w-full tablet:u-w-72 u-drop-shadow">{createFolder}</a>
                            </Link>
                        }
                        {(folderId && hasAddFileAction) &&
                            <Link href={{
                                pathname: `${groupBasePath}/files/create`,
                                query: { 
                                    folderId: folderId 
                                }
                            }}>
                                <a className="c-button c-button--outline u-w-full tablet:u-w-72 u-drop-shadow">{createFile}</a>
                            </Link>
                        }
                    </p>
                }
                {hasFolderContents &&
                    <>
                        <AriaLiveRegion>
                            <DataGrid
                                id="group-table-files"
                                text={{
                                    caption: 'Group folders'
                                }}
                                columnList={[
                                    {
                                        children: 'Type',
                                        className: 'u-text-center'
                                    },
                                    {
                                        children: 'Name'
                                    },
                                    {
                                        children: 'Description'
                                    },
                                    {
                                        children: 'Modified'
                                    },
                                    {
                                        children: 'Actions'
                                    }
                                ]}
                                rowList={gridRowList} />
                        </AriaLiveRegion>
                        <PaginationWithStatus
                            id="file-list-pagination"
                            shouldEnableLoadMore={true}
                            getPageAction={handleGetPage}
                            {...dynamicPagination} />
                    </>
                }
            </LayoutColumn>
        </>

    )

}
