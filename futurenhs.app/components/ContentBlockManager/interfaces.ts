import { CmsContentBlock } from "@appTypes/contentBlock";
import { FormConfig } from '@appTypes/form';
import { cprud } from '@constants/cprud'
import { FormErrors } from "@appTypes/form";

export interface Props {
    csrfToken: string;
    forms: Record<string, FormConfig>;
    blocks: Array<CmsContentBlock>;
    blocksTemplate: Array<CmsContentBlock>;
    initialState?: cprud;
    blocksChangeAction?: (blocks: Array<CmsContentBlock>) => void
    stateChangeAction?: (currentState: cprud) => void
    createBlockAction?: (blockTypeId: string) => void
    saveBlocksAction?: (blocks: Array<CmsContentBlock>) => Promise<FormErrors>
    themeId: string;
    className?: string;
}
