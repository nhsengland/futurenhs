export interface Props {
    typeId: string;
    instanceId: string | number;
    isEditable?: boolean;
    isTemplate?: boolean;
    shouldRenderMovePrevious?: boolean;
    shouldRenderMoveNext?: boolean;
    children: any;
    text: {
        name: string;
    };
    createAction?: (id: string | number) => void;
    deleteAction?: (id: string | number) => void;
    movePreviousAction?: (id: string | number) => void;
    moveNextAction?: (id: string | number) => void;
    className?: string
}
