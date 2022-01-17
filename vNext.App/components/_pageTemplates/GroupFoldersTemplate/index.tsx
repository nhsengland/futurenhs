import { useMemo, useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/router';

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
    content,
    image,
    folderId,
    folder,
    files,
    pagination
}) => {

    const router = useRouter();
    const [fileList, setFileList] = useState(files);
    const [dynamicPagination, setPagination] = useState(pagination);

    const { 
        id, 
        name, 
        path, 
        bodyHtml 
    } = folder ?? {};
    
    const hasFolders: boolean = files?.length > 0;

    const groupBasePath: string = getRouteToParam({
        router: router,
        paramName: routeParams.GROUPID,
        shouldIncludeParam: true
    });

    const folderBasePath: string = folderId ? getRouteToParam({
        router: router,
        paramName: routeParams.FOLDERID
    }) : `${groupBasePath}/folders`;

    const breadCrumbList: BreadCrumbList = []; 
    
    path?.forEach(({ element, text }) => {

        if(element !== id){

            breadCrumbList.push({
                element: `${folderBasePath}/${element}`,
                text: text
            });

        }

    });

    const gridRowList = useMemo(() => fileList?.map(({ 
        id,
        type, 
        name, 
        bodyHtml, 
        modified, 
        modifiedBy, 
        createdBy }) => {

            const isFolder: boolean = type === 'folder';
            const href: string = `${folderBasePath}/${encodeURIComponent(id)}`;

            return [
                {
                    children: <SVGIcon name={`icon-${type}`} className="u-w-4 u-h-6" />,
                    className: 'u-text-center u-text-base'
                },
                {
                    children: <Link href={href}>{name}</Link>,
                    className: 'u-w-1/6'
                },
                {
                    children: <RichText bodyHtml={bodyHtml} wrapperElementType='span' className='o-truncated-text-lines-3' />
                },
                {
                    children: isFolder ? '' : <RichText bodyHtml={`<p class='u-mb-1'>${dateTime({})(modified)}</p><p class='u-mb-1'><span class='u-text-bold'>By</span> ${modifiedBy}</p><p><span class='u-text-bold'>Author</span> ${createdBy}</p>`} />,
                    className: 'u-w-1/4'
                },
                {
                    children: isFolder ? '' : <><SVGIcon name="icon-download" className="u-w-4 u-h-6 u-mr-2 u-fill-theme-8" /><a href="/" className="u-align-top">Download</a></>,
                    className: 'u-w-1/6'
                }
            ]
        
    }), [fileList, folderId]);

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

        setFileList([...fileList, ...additionalFiles]);
        setPagination(pagination);

    };

    return (

        <GroupLayout 
            id="files"
            user={user}
            content={content}
            image={image} 
            className="u-bg-theme-3">
                <LayoutColumn className="c-page-body">
                    <BreadCrumb 
                        content={{
                            ariaLabelText: 'Folders'
                        }}
                        breadCrumbList={breadCrumbList}
                        className="u-mb-10" />
                    {folderId &&
                        <>
                            <LayoutColumnContainer>
                                <LayoutColumn tablet={6} desktop={8} className="u-self-center">
                                    <h2>{name}</h2>
                                </LayoutColumn>
                                <LayoutColumn tablet={6} desktop={4} className="tablet:u-text-right">
                                    <p className="u-mb-0">
                                        <Link href={`${groupBasePath}/folders/create`}>
                                            <a className="c-button c-button--outline u-mr-2 u-drop-shadow">Delete folder</a>
                                        </Link>
                                        <a href="/" className="c-button c-button--outline u-drop-shadow">Edit folder</a>
                                    </p> 
                                </LayoutColumn>
                            </LayoutColumnContainer>
                            <hr />
                        </>
                    }
                    <RichText wrapperElementType="p" bodyHtml={bodyHtml} />
                    <p className="u-mb-10">
                        <Link href={`${groupBasePath}/folders/create`}>
                            <a className="c-button c-button--outline u-mr-2 u-w-72 u-drop-shadow">Add folder</a>
                        </Link>
                        {folderId &&
                            <a href="/" className="c-button c-button--outline u-min-w-70 u-w-72 u-drop-shadow">Upload file</a>
                        }
                    </p>
                    {hasFolders &&
                        <>
                            <AriaLiveRegion>
                                <DataGrid 
                                    content={{
                                        captionHtml: 'Group folders'
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
