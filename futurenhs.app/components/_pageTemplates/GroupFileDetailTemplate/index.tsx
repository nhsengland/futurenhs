import classNames from 'classnames';
import { useRouter } from 'next/router';

import { Link } from '@components/Link';
import { dateTime } from '@helpers/formatters/dateTime';
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
 * Group file detail template
 */
export const GroupFileDetailTemplate: (props: Props) => JSX.Element = ({
    fileId,
    file,
    contentText
}) => {

    const router = useRouter();

    const { createdByLabel } = contentText ?? {};
    const { path, name, createdBy, modified, modifiedBy, downloadLink, text: fileText } = file ?? {};
    const { body } = fileText ?? {};

    const breadCrumbList: BreadCrumbList = [];

    const groupBasePath: string = getRouteToParam({
        router: router,
        paramName: routeParams.GROUPID,
        shouldIncludeParam: true
    });

    if (path?.length > 0) {

        breadCrumbList.push({
            element: `${groupBasePath}/folders`,
            text: 'Files'
        });

        path?.forEach(({ element, text }) => {

            if (element !== fileId) {

                breadCrumbList.push({
                    element: `${groupBasePath}/folders/${element}`,
                    text: text
                });

            }

        });

    }

    const hasBreadCrumb: boolean = breadCrumbList.length > 0;

    const generatedCellClasses = {
        name: classNames({
            ['u-justify-between u-w-full tablet:u-w-1/6']: true
        }),
        modifiedBy: classNames({
            ['u-justify-between u-w-full tablet:u-w-1/4']: true
        }),
        lastUpdate: classNames({
            ['u-justify-between u-w-full u-w-1/6 tablet:u-w-8 u-items-center']: true
        }),
        actions: classNames({
            ['u-justify-between u-w-full tablet:u-w-1/6 tablet:u-justify-end tablet:u-text-right']: true
        })
    };

    const generatedHeaderCellClasses = {
        name: classNames({
            ['u-text-bold']: true
        }),
        modifiedBy: classNames({
            ['u-text-bold']: true
        }),
        lastUpdate: classNames({
            ['u-text-bold']: true
        }),
        actions: classNames({
            ['u-hidden']: true
        })
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
                <h2 className="nhsuk-heading-l">{name}</h2>
                <hr />
                <RichText wrapperElementType="p" bodyHtml={body} />
                {createdBy &&
                    <p className="u-mb-14">
                        <span className="u-text-bold u-mr-6">{createdByLabel}</span>
                        <Link href={`${groupBasePath}/members/${createdBy.id}`}>{createdBy.text.userName}</Link>
                    </p>
                }
                <DataGrid
                    id="group-table-file"
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
                            children: 'Actions',
                            className: 'tablet:u-text-right'
                        }
                    ]}
                    rowList={[
                        [
                            {
                                children: name,
                                className: generatedCellClasses.name,
                                headerClassName: generatedHeaderCellClasses.name
                            },
                            {
                                children: modifiedBy?.text?.userName ?? '',
                                className: generatedCellClasses.modifiedBy,
                                headerClassName: generatedHeaderCellClasses.modifiedBy
                            },
                            {
                                children: modified ? dateTime({ value: modified }) : '',
                                className: generatedCellClasses.lastUpdate,
                                headerClassName: generatedHeaderCellClasses.lastUpdate
                            },
                            {
                                children: <><SVGIcon name="icon-download" className="u-w-4 u-h-6 u-mr-2 u-align-middle u-fill-theme-8" /><a href={downloadLink}>Download</a></>,
                                className: generatedCellClasses.actions,
                                headerClassName: generatedHeaderCellClasses.actions
                            }
                        ]
                    ]}
                    className="u-mb-12" />
            </LayoutColumn>
        </>

    )

}
