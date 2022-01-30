import classNames from 'classnames';

import { Props } from './interfaces';

export const DataGrid: (props: Props) => JSX.Element = ({ 
    text,
    columnList = [],
    rowList = [],
    id,
    shouldRenderCaption,
    className
}) => {

    const shouldRenderAdvancedVariant: boolean = columnList.length > 2;

    const { caption } = text ?? {}; 

    const generatedClasses = {
        wrapper: classNames('c-table', className),
        caption: classNames('c-table_caption', {
            ['u-sr-only']: !shouldRenderCaption
        }),
        head: classNames({
            ['u-sr-only']: shouldRenderAdvancedVariant, 
            ['tablet:u-not-sr-only']: shouldRenderAdvancedVariant
        }),
        bodyRow: classNames({
            ['u-block']: shouldRenderAdvancedVariant,
            ['tablet:u-table-row']: shouldRenderAdvancedVariant,
            ['u-table-row']: !shouldRenderAdvancedVariant,
        })
    };

    return (

        <table id={id} className={generatedClasses.wrapper}>
            <caption className={generatedClasses.caption}>{caption}</caption>
            <thead role="rowgroup" className={generatedClasses.head}>
                <tr role="row">
                    {columnList.map(({ children, className }, index) => <th key={index} role="columnheader" scope="col" className={className}>{children}</th>)}
                </tr>
            </thead>
            <tbody>
                {rowList.map((row, index) => {

                    return (
                    
                        <tr key={index} role="row" className={generatedClasses.bodyRow}>
                            {row.map(({ children, className }, index) => {

                                const generatedCellClasses = {
                                    bodyCell: classNames(className, {
                                        ['u-flex']: shouldRenderAdvancedVariant,
                                        ['tablet:u-table-cell']: shouldRenderAdvancedVariant,
                                        ['u-table-cell']: !shouldRenderAdvancedVariant,
                                        ['u-justify-between']: shouldRenderAdvancedVariant
                                    })
                                };
                            
                                return (

                                    <td 
                                        key={index} 
                                        role="cell" 
                                        className={generatedCellClasses.bodyCell}>
                                            <span className="tablet:u-hidden">{columnList[index].children} </span>{children}
                                    </td>

                                )
                                                                
                            })}
                        </tr>
                    
                    )
                              
                })}
            </tbody>
        </table>

    );

}

