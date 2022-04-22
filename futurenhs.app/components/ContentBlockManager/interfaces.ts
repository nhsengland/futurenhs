import { CmsContentBlock } from "@appTypes/contentBlock";
import { cprud } from '@constants/cprud'

export interface Props {
    blocks: Array<CmsContentBlock>;
    blocksTemplate: Array<CmsContentBlock>;
    currentState: cprud;
    blocksChangeAction?: (blocks: Array<CmsContentBlock>) => void
    stateChangeAction?: (currentState: cprud) => void
    createBlockAction?: (blockTypeId: string) => void
    className?: string
}
