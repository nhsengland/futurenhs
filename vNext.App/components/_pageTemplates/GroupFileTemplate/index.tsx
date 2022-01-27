import { useState, useEffect } from 'react';
import { useRouter } from 'next/router';

import { Link } from '@components/Link';
import { dateTime } from '@helpers/formatters/dateTime';
import { AriaLiveRegion } from '@components/AriaLiveRegion';
import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { LayoutColumn } from '@components/LayoutColumn';
import { DataGrid } from '@components/DataGrid';
import { RichText } from '@components/RichText';
import { BreadCrumb } from '@components/BreadCrumb';
import { getRouteToParam } from '@helpers/routing/getRouteToParam';
import { routeParams } from '@constants/routes';
import { BreadCrumbList } from '@appTypes/routing';

import { Props } from './interfaces';

/**
 * Group file detail template
 */
export const GroupFileTemplate: (props: Props) => JSX.Element = ({
    user,
    actions,
    fileId,
    file,
    text,
    image
}) => {

    const router = useRouter();
    const [shouldRenderFilePreview, setShouldRenderFilePreview] = useState(false);

    const { path, name, createdBy, text: fileText } = file ?? {};
    const { body } = fileText ?? {};

    const breadCrumbList: BreadCrumbList = [];

    const groupBasePath: string = getRouteToParam({
        router: router,
        paramName: routeParams.GROUPID,
        shouldIncludeParam: true
    });

    if(path?.length > 0){

        breadCrumbList.push({
            element: `${groupBasePath}/folders`,
            text: 'Files'
        });

        path?.forEach(({ element, text }) => {

            if(element !== fileId){
    
                breadCrumbList.push({
                    element: `${groupBasePath}/folders/${element}`,
                    text: text
                });
    
            }
    
        });

    }

    const hasBreadCrumb: boolean = breadCrumbList.length > 0;

    useEffect(() => {

        setShouldRenderFilePreview(true);

    }, []);

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
                    <h2>{name}</h2>
                    <hr />
                    <RichText wrapperElementType="p" bodyHtml={body} />
                    {createdBy &&
                        <p className="u-mb-14">
                            <span className="u-text-bold u-mr-6">Owner</span>
                            <Link href={`${groupBasePath}/members/${createdBy.id}`}>{createdBy.name}</Link>
                        </p>
                    }
                    <DataGrid
                        text={{
                            caption: 'File data'
                        }} 
                        shouldRenderCaption={true}
                        columnList={[
                            {
                                children: 'Name'
                            },
                            {
                                children: 'Modified by'
                            },
                            {
                                children: 'Last update'
                            },
                            {
                                children: 'Actions'
                            }
                        ]}
                        rowList={[
                            [
                                {
                                    children: 'File name'
                                },
                                {
                                    children: dateTime({})('2021-06-04T11:47:30Z')
                                },
                                {
                                    children: dateTime({})('2021-12-10T02:16:03Z')
                                },
                                {
                                    children: <a href="/">Download</a>
                                }
                            ]
                        ]} 
                        className="u-mb-12" />
                    <AriaLiveRegion>
                        {shouldRenderFilePreview &&
                            <>
                                <h2>File preview</h2>
                                <iframe src="https://www.bbc.co.uk" className="u-w-full"></iframe> 
                            </>
                        }
                    </AriaLiveRegion>
                </LayoutColumn>
        </GroupLayout>

    )

}