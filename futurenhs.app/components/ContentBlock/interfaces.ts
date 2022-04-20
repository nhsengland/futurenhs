export interface Props {
    blockTypeId: string;
    blockInstanceId: string;
    isEditable?: boolean;
    shouldRenderMovePrevious?: boolean;
    shouldRenderMoveNext?: boolean;
    children: any;
    text: {
        name: string;
    };
    createAction?: (id: string) => void;
    deleteAction?: (id: string) => void;
    movePreviousAction?: (id: string) => void;
    moveNextAction?: (id: string) => void;
    className?: string
}
