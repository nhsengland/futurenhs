export type DataCell = {
    children: any;
    className?: string;
};

export type DataRow = Array<DataCell>;

export interface Props {
    columnList: DataRow;
    rowList: Array<DataRow>
    content: {
        captionHtml: string;
    };
    shouldRenderCaption?: boolean;
    id?: string;
    className?: string;
}
