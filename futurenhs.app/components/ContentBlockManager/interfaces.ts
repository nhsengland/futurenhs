import { ContentBlock } from "@appTypes/contentBlock";
import { crud } from '@constants/crud'

export interface Props {
    blocks: Array<ContentBlock>;
    templateBlocks: Array<ContentBlock>;
    currentState: crud;
    stateChangeAction?: (currentState: crud) => void
    createBlockAction?: (blockTypeId: string) => void
    className?: string
}
