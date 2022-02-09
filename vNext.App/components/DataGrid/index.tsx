import classNames from 'classnames';

import { DynamicListContainer } from '@components/DynamicListContainer';

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
            ['u-sr-only tablet:u-not-sr-only']: shouldRenderAdvancedVariant
        }),
        bodyRow: classNames({
            ['u-flex u-flex-wrap tablet:u-table-row u-mb-3 tablet:u-mb-0']: shouldRenderAdvancedVariant,
            ['u-table-row']: !shouldRenderAdvancedVariant
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
            <DynamicListContainer containerElementType="tbody" shouldFocusLatest={true}>
                {rowList.map((row, index) => {

                    return (
                    
                        <tr key={index} role="row" className={generatedClasses.bodyRow}>
                            {row.map(({ 
                                children, 
                                className, 
                                headerClassName 
                            }, index) => {

                                const generatedCellClasses = {
                                    bodyCell: classNames(className, {
                                        ['u-flex']: shouldRenderAdvancedVariant,
                                        ['tablet:u-table-cell']: shouldRenderAdvancedVariant,
                                        ['u-table-cell']: !shouldRenderAdvancedVariant
                                    }),
                                    bodyCellLabel: classNames(headerClassName, {
                                        ['tablet:u-hidden']: true
                                    })
                                };
                            
                                return (

                                    <td 
                                        key={index} 
                                        role="cell" 
                                        className={generatedCellClasses.bodyCell}>
                                            <span className={generatedCellClasses.bodyCellLabel}>{columnList[index].children} </span>{children}
                                    </td>

                                )
                                                                
                            })}
                        </tr>
                    
                    )
                            
                })}
            </DynamicListContainer>
        </table>

    );

}

