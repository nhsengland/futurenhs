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
import { getGroupFolders } from '@services/getGroupFolders';
import { getRouteToParam } from '@helpers/routing/getRouteToParam';
import { BreadCrumbList } from '@appTypes/routing';

import { Props } from './interfaces';

/**
 * Group folders template
 */
export const GroupFoldersTemplate: (props: Props) => JSX.Element = ({
    user,
    groupId,
    text,
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
            name, 
            path, 
            bodyHtml } = folder ?? {};
    
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
            const iconName: string = extension?.split('.')?.[1] ?? (isFolder ? 'folder' : '');
            const href: string = `${isFolder ? folderBasePath : fileBasePath}/${encodeURIComponent(id)}`;

            return [
                {
                    children: <SVGIcon name={`icon-${iconName}`} className="u-w-4 u-h-6" />,
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
                    children: isFolder ? '' : <RichText bodyHtml={`<p class='u-mb-1'>${dateTime({})(modified)}</p>${modifiedBy && '<p class="u-mb-1"><span class="u-text-bold">By</span> ' + modifiedBy + '</p>'}${createdBy && '<p><span class="u-text-bold">Author</span> ' + createdBy + '</p>'}`} />,
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

        const { data: additionalFiles, pagination } = await getGroupFolders({
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
            text={text}
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
                                                <a className="c-button c-button--outline u-mr-2 u-drop-shadow">Delete folder</a>
                                            </Link>
                                            <a href="/" className="c-button c-button--outline u-drop-shadow">Edit folder</a>
                                        </p> 
                                    </LayoutColumn>
                                }
                            </LayoutColumnContainer>
                            <hr />
                        </>
                    }
                    {!folderId &&
                        <h2>Files</h2>
                    }
                    {bodyHtml &&
                        <RichText wrapperElementType="p" bodyHtml={bodyHtml} />
                    }
                    {(hasAddFolderAction || hasAddFileAction) &&
                        <p className="u-mb-10">
                            {hasAddFolderAction &&
                                <Link href={`${groupBasePath}/folders/create`}>
                                    <a className="c-button c-button--outline u-mr-2 u-w-72 u-drop-shadow">Add folder</a>
                                </Link>
                            }
                            {(folderId && hasAddFileAction) &&
                                <a href="/" className="c-button c-button--outline u-min-w-70 u-w-72 u-drop-shadow">Upload file</a>
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
