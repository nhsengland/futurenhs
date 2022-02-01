import { useMemo, useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/router';

import { actions as userActions } from '@constants/actions';
import { routeParams } from '@constants/routes';
import { BreadCrumb } from '@components/BreadCrumb';
import { SVGIcon } from '@components/SVGIcon';
import { dateTime } from '@helpers/formatters/dateTime';
import { AriaLiveRegion } from '@components/AriaLiveRegion';
import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
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
    entityText,
    image,
    folderId,
    folder,
    folderContents,
    actions,
    pagination
}) => {

    const router = useRouter();
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

    const breadCrumbList: BreadCrumbList = [];

    if(path?.length > 0){

        breadCrumbList.push({
            element: folderBasePath,
            text: 'Files'
        });

    }
    
    path?.forEach(({ element, text }) => {

        if(element !== id){

            breadCrumbList.push({
                element: `${groupBasePath}/folders/${element}`,
                text: text
            });

        }

    });

    const hasBreadCrumb: boolean = breadCrumbList.length > 0;

    const iconMap = {
        ['.doc']: 'icon-docx',
        ['.docx']: 'icon-docx',
        ['.xls']: 'icon-xls',
        ['.xlsx']: 'icon-xls',
        ['.ppt']: 'icon-ppt',
        ['.pptx']: 'icon-ppt',
        ['.pdf']: 'icon-pdf',
        ['.txt']: 'icon-document'
    };

    const gridRowList = useMemo(() => folderContentsList?.map(({ 
        id,
        type, 
        extension,
        name, 
        text, 
        modified, 
        modifiedBy, 
        createdBy }) => {

            const { body } = text ?? {};

            const isFolder: boolean = type === 'folder';
            const href: string = `${isFolder ? folderBasePath : fileBasePath}/${encodeURIComponent(id)}`;

            let iconName: string = isFolder ? 'icon-folder' : iconMap[extension];

            return [
                {
                    children: <><SVGIcon name={iconName} className="u-w-4 u-h-6" /><span className="u-block u-text-bold">{extension}</span></>,
                    className: 'u-text-center u-text-base'
                },
                {
                    children: <Link href={href}><a className="o-truncated-text-lines-3">{name}</a></Link>,
                    className: 'u-w-1/6'
                },
                {
                    children: <RichText bodyHtml={body} wrapperElementType='span' className='o-truncated-text-lines-3' />
                },
                {
                    children: isFolder ? '' : <RichText bodyHtml={`<p class='u-mb-1'>${dateTime({})(modified)}</p>${modifiedBy?.text?.userName && '<p class="u-mb-1"><span class="u-text-bold">By</span> ' + modifiedBy.text.userName + '</p>'}${createdBy?.text?.userName && '<p><span class="u-text-bold">Author</span> ' + createdBy.text.userName + '</p>'}`} />,
                    className: 'u-w-1/4'
                },
                {
                    children: isFolder ? '' : <><SVGIcon name="icon-download" className="u-w-4 u-h-6 u-mr-2 u-fill-theme-8" /><a href="/" className="u-align-top">Download</a></>,
                    className: 'u-w-1/6'
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

    return (

        <GroupLayout 
            id="files"
            user={user}
            actions={actions}
            text={entityText}
            image={image} 
            className="u-bg-theme-3">
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
                                    <h2 className="u-m-0 o-truncated-text-lines-3">{name}</h2>
                                </LayoutColumn>
                                {(folderId && (hasEditFolderAction || hasDeleteFolderAction)) &&
                                    <LayoutColumn tablet={6} desktop={4} className="tablet:u-text-right">
                                        <p className="u-mb-0">
                                            <Link href={`${groupBasePath}/folders/create`}>
                                                <a className="c-button c-button--outline u-mr-2 u-drop-shadow">{deleteFolder}</a>
                                            </Link>
                                            <a href="/" className="c-button c-button--outline u-drop-shadow">{updateFolder}</a>
                                        </p> 
                                    </LayoutColumn>
                                }
                            </LayoutColumnContainer>
                            <hr />
                        </>
                    }
                    {!folderId &&
                        <h2>{foldersHeading}</h2>
                    }
                    {body &&
                        <RichText wrapperElementType="p" bodyHtml={body} />
                    }
                    {(!hasFolderContents && !folderId) &&
                        <p className="u-mb-12">{noFolders}</p>
                    }
                    {(hasAddFolderAction || hasAddFileAction) &&
                        <p className="u-mb-10">
                            {hasAddFolderAction &&
                                <Link href={`${groupBasePath}/folders/create`}>
                                    <a className="c-button c-button--outline u-mr-2 u-w-72 u-drop-shadow">{createFolder}</a>
                                </Link>
                            }
                            {(folderId && hasAddFileAction) &&
                                <a href="/" className="c-button c-button--outline u-min-w-70 u-w-72 u-drop-shadow">{createFile}</a>
                            }
                        </p>
                    }
                    {hasFolderContents && 
                        <>
                            <AriaLiveRegion>
                                <DataGrid 
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
                                            children: 'Download'
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
        </GroupLayout>

    )

}
