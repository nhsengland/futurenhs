import { Link } from '@components/Link';
import { AriaLiveRegion } from '@components/AriaLiveRegion';
import { CollaboraFilePreview } from '@components/CollaboraFilePreview';
import { NoScript } from '@components/NoScript';
import { LayoutColumn } from '@components/LayoutColumn';
import { BreadCrumb } from '@components/BreadCrumb';
import { SVGIcon } from '@components/SVGIcon';
import { BreadCrumbList } from '@appTypes/routing';

import { Props } from './interfaces';

/**
 * Group file preview template
 */
export const GroupFilePreviewTemplate: (props: Props) => JSX.Element = ({
    csrfToken,
    fileId,
    file,
    preview,
    routes
}) => {

    const { path, name } = file ?? {};
    const { accessToken, wopiClientUrl } = preview ?? {};

    const breadCrumbList: BreadCrumbList = [];
    const hasCollaboraData: boolean = Boolean(accessToken) && Boolean(wopiClientUrl);
    const fileDetailPath: string = `${routes.groupFilesRoot}/${fileId}/detail`;

    console.log(preview);

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
                <NoScript 
                    headingLevel={3} 
                    text={{
                        heading: 'Important',
                        body: 'JavaScript must be enabled in your browser to view this file'
                    }} />
                <AriaLiveRegion>
                    {hasCollaboraData &&
                        <CollaboraFilePreview 
                            csrfToken={csrfToken} 
                            accessToken={accessToken} 
                            wopiClientUrl={wopiClientUrl} />
                    }
                </AriaLiveRegion>
                <p>
                    <Link href={fileDetailPath}><a><SVGIcon name="icon-view" className="u-w-4 u-h-6 u-mr-2 u-align-middle u-fill-theme-8" />View details</a></Link>
                </p>
            </LayoutColumn>
        </>

    )

}
