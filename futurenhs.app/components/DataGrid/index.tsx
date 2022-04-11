import classNames from 'classnames'

import { DynamicListContainer } from '@components/DynamicListContainer'

import { Props } from './interfaces'

export const DataGrid: (props: Props) => JSX.Element = ({
    text,
    columnList = [],
    rowList = [],
    id,
    shouldRenderCaption,
    className,
}) => {
    const shouldRenderAdvancedVariant: boolean = columnList.length > 2

    const { caption } = text ?? {}

    const generatedClasses = {
        wrapper: classNames('c-table', className),
        caption: classNames('c-table_caption', {
            ['u-sr-only']: !shouldRenderCaption,
        }),
        head: classNames({
            ['u-sr-only tablet:u-not-sr-only']: shouldRenderAdvancedVariant,
        }),
        bodyRow: classNames({
            ['u-flex u-flex-wrap tablet:u-table-row u-mb-3 tablet:u-mb-0']:
                shouldRenderAdvancedVariant,
            ['u-table-row']: !shouldRenderAdvancedVariant,
        }),
    }

    return (
        <table id={id} className={generatedClasses.wrapper}>
            <caption className={generatedClasses.caption}>{caption}</caption>
            <thead className={generatedClasses.head}>
                <tr>
                    {columnList.map(({ children, className }, index) => (
                        <th key={index} scope="col" className={className}>
                            {children}
                        </th>
                    ))}
                </tr>
            </thead>
            <DynamicListContainer
                containerElementType="tbody"
                shouldEnableLoadMore={true}
            >
                {rowList.map((row, index) => {
                    return (
                        <tr key={index} className={generatedClasses.bodyRow}>
                            {row.map(
                                (
                                    {
                                        children,
                                        className,
                                        headerClassName,
                                        shouldRenderCellHeader,
                                    },
                                    index
                                ) => {
                                    const generatedCellClasses = {
                                        bodyCell: classNames(className, {
                                            ['u-flex']:
                                                shouldRenderAdvancedVariant,
                                            ['tablet:u-table-cell']:
                                                shouldRenderAdvancedVariant,
                                            ['u-table-cell']:
                                                !shouldRenderAdvancedVariant,
                                        }),
                                        bodyCellLabel: classNames(
                                            headerClassName,
                                            {
                                                ['tablet:u-hidden']: true,
                                            }
                                        ),
                                    }

                                    return (
                                        <td
                                            key={index}
                                            className={
                                                generatedCellClasses.bodyCell
                                            }
                                        >
                                            {shouldRenderCellHeader && (
                                                <span
                                                    aria-hidden={true}
                                                    className={
                                                        generatedCellClasses.bodyCellLabel
                                                    }
                                                >
                                                    {columnList[index].children}{' '}
                                                </span>
                                            )}
                                            {children}
                                        </td>
                                    )
                                }
                            )}
                        </tr>
                    )
                })}
            </DynamicListContainer>
        </table>
    )
}
