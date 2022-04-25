import { CmsContentBlock } from "@appTypes/contentBlock";
import { FormConfig } from '@appTypes/form';
import { cprud } from '@constants/cprud'

export interface Props {
    csrfToken: string;
    forms: Record<string, FormConfig>;
    blocks: Array<CmsContentBlock>;
    blocksTemplate: Array<CmsContentBlock>;
    currentState: cprud;
    blocksChangeAction?: (blocks: Array<CmsContentBlock>) => void
    stateChangeAction?: (currentState: cprud) => void
    createBlockAction?: (blockTypeId: string) => void
    className?: string
}
