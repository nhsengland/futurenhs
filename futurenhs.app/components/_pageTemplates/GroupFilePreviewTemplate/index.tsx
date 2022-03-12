import { useState, useEffect } from 'react';
import classNames from 'classnames';
import { useRouter } from 'next/router';

import { Link } from '@components/Link';
import { dateTime } from '@helpers/formatters/dateTime';
import { AriaLiveRegion } from '@components/AriaLiveRegion';
import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { LayoutColumn } from '@components/LayoutColumn';
import { DataGrid } from '@components/DataGrid';
import { RichText } from '@components/RichText';
import { BreadCrumb } from '@components/BreadCrumb';
import { SVGIcon } from '@components/SVGIcon';
import { getRouteToParam } from '@helpers/routing/getRouteToParam';
import { routeParams } from '@constants/routes';
import { BreadCrumbList } from '@appTypes/routing';

import { Props } from './interfaces';

/**
 * Group file preview template
 */
export const GroupFilePreviewTemplate: (props: Props) => JSX.Element = ({
    fileId,
    file,
    routes,
    contentText
}) => {

    const router = useRouter();
    const [shouldRenderFilePreview, setShouldRenderFilePreview] = useState(false);

    const { path, name, text: fileText } = file ?? {};

    const breadCrumbList: BreadCrumbList = [];

    const fileDetailPath: string = `${routes.groupFilesRoot}/${fileId}/detail`;

    if (path?.length > 0) {

        breadCrumbList.push({
            element: `${routes.groupFoldersRoot}`,
            text: 'Files'
        });

        path?.forEach(({ element, text }) => {

            if (element !== fileId) {

                breadCrumbList.push({
                    element: `${routes.groupFoldersRoot}/${element}`,
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

        <>
            <LayoutColumn className="c-page-body u-pt-4">
                {hasBreadCrumb &&
                    <BreadCrumb
                        text={{
                            ariaLabel: 'Folders'
                        }}
                        breadCrumbList={breadCrumbList}
                        shouldLinkCrumbs={false}
                        className="u-text-lead u-mb-10 u-fill-theme-0" />
                }
                <h2 className="nhsuk-heading-l">{name}</h2>
                <hr />
                <AriaLiveRegion>
                    {shouldRenderFilePreview &&
                        <>
                            <iframe src="https://www.bbc.co.uk" className="u-w-full" style={{
                                height: '1000px'                            
                            }}></iframe>
                        </>
                    }
                </AriaLiveRegion>
                <p>
                    <Link href={fileDetailPath}><a><SVGIcon name="icon-view" className="u-w-4 u-h-6 u-mr-2 u-align-middle u-fill-theme-8" />View details</a></Link>
                </p>
            </LayoutColumn>
        </>

    )

}
