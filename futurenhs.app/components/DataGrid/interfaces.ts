export type DataCell = {
    children: any
    className?: string
    headerClassName?: string
    shouldRenderCellHeader?: boolean
}

export type DataRow = Array<DataCell>

export interface Props {
    /**
     * Generates table columns
     */
    columnList: DataRow
    /**
     * Generates table rows
     */
    rowList: Array<DataRow>
    /**
     * Controls page text content
     */
    text: {
        caption: string
    }
    /**
     * Determines whether the table caption should be visible
     */
    shouldRenderCaption?: boolean
    id?: string
    className?: string
}
