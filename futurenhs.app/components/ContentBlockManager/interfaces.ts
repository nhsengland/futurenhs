export interface Props {
    blocks: Array<any>;
    stateChangeAction?: (currentState: 'create' | 'update') => void
    createBlockAction?: (blockTypeId: string) => void
    className?: string
}
