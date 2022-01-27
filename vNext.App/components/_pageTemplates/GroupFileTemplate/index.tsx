import { useState, useEffect } from 'react';

import { Link } from '@components/Link';
import { dateTime } from '@helpers/formatters/dateTime';
import { AriaLiveRegion } from '@components/AriaLiveRegion';
import { GroupLayout } from '@components/_pageLayouts/GroupLayout';
import { LayoutColumn } from '@components/LayoutColumn';
import { DataGrid } from '@components/DataGrid';
import { RichText } from '@components/RichText';

import { Props } from './interfaces';

/**
 * Group file detail template
 */
export const GroupFileTemplate: (props: Props) => JSX.Element = ({
    user,
    actions,
    file,
    text,
    image
}) => {

    const [shouldRenderFilePreview, setShouldRenderFilePreview] = useState(false);

    const { text: fileText } = file ?? {};
    const { name, body } = fileText ?? {};

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
                    <h2>{name}</h2>
                    <hr />
                    <RichText wrapperElementType="p" bodyHtml={body} />
                    <p className="u-mb-14">
                        <span className="u-text-bold u-mr-6">Owner</span>
                        <Link href="/members/todo">Jane Richardson</Link>
                    </p>
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
