import { CmsContentBlock } from "@appTypes/contentBlock";
import { FormConfig } from '@appTypes/form';
import { cprud } from '@constants/cprud'
import { FormErrors } from "@appTypes/form";

export interface Props {
    blocks: Array<CmsContentBlock>;
    blocksTemplate: Array<CmsContentBlock>;
    initialState?: cprud;
    text: {
        headerReadBody?: string;
        headerPreviewBody?: string;
        headerCreateHeading?: string;
        headerCreateBody?: string;
        headerUpdateHeading?: string;
        headerUpdateBody?: string;
        headerEnterUpdateButton?: string;
        headerLeaveUpdateButton?: string;
        headerDiscardUpdateButton?: string;
        headerPreviewUpdateButton?: string;
        headerPublishUpdateButton?: string;
        createButton: string;
        cancelCreateButton: string;
    };
    shouldRenderEditingHeader?: boolean;
    discardUpdateAction?: () => void;
    blocksChangeAction?: (blocks: Array<CmsContentBlock>) => void
    stateChangeAction?: (currentState: cprud) => void
    createBlockAction?: (blockTypeId: string) => void
    saveBlocksAction?: (blocks: Array<CmsContentBlock>, localErrors: FormErrors) => Promise<FormErrors>
    themeId: string;
    className?: string;
}
