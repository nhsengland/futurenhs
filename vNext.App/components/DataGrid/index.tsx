import classNames from 'classnames';

import { Props } from './interfaces';

export const DataGrid: (props: Props) => JSX.Element = ({ 
    content,
    columnList = [],
    rowList = [],
    id,
    shouldRenderCaption,
    className
}) => {

    const hasColumnList: boolean = columnList.length > 0;

    const { captionHtml } = content ?? {}; 

    const generatedClasses = {
        wrapper: classNames('c-table', className),
        caption: classNames('c-table_caption', {
            ['u-sr-only']: !shouldRenderCaption
        })
    }

    return (

        <table id={id} className={generatedClasses.wrapper}>
            <caption className={generatedClasses.caption}>{captionHtml}</caption>
            {hasColumnList &&
                <thead>
                    <tr>
                        {columnList.map(({ children, className }, index) => <th key={index} className={className}>{children}</th>)}
                    </tr>
                </thead>
            }
            <tbody>
                {rowList.map((row, index) => {

                    return (
                    
                        <tr key={index}>
                            {row.map(({ children, className }, index) => <td key={index} className={className}>{children}</td>)}
                        </tr>
                    
                    )
                              
                })}
            </tbody>
        </table>

    );

}

