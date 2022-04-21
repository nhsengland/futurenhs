import { ContentBlock } from "@appTypes/contentBlock";

export interface Props {
    blocks: Array<ContentBlock>;
    stateChangeAction?: (currentState: 'create' | 'update') => void
    createBlockAction?: (blockTypeId: string) => void
    className?: string
}
