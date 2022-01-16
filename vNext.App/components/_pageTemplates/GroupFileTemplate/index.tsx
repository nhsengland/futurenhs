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
    content,
    image
}) => {

    const [shouldRenderFilePreview, setShouldRenderFilePreview] = useState(false);

    useEffect(() => {

        setShouldRenderFilePreview(true);

    }, []);

    return (

        <GroupLayout 
            id="files"
            user={user}
            content={content}
            image={image} 
            className="u-bg-theme-3">
                <LayoutColumn className="c-page-body">
                    <h2>File title</h2>
                    <hr />
                    <RichText wrapperElementType="p" bodyHtml="The description of the file added by the user when uploading the document. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat." />
                    <p className="u-mb-14">
                        <span className="u-text-bold u-mr-6">Owner</span>
                        <Link href="/members/todo">Jane Richardson</Link>
                    </p>
                    <DataGrid
                        content={{
                            captionHtml: 'File data'
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
                                children: 'Download'
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
