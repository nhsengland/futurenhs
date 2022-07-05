import { CmsContentBlock } from '@appTypes/cmsContent'
import { cprud } from '@constants/cprud'
import { FormErrors } from '@appTypes/form'

export interface Props {
    activeBlocks: Array<CmsContentBlock>
    referenceBlocks: Array<CmsContentBlock>
    blocksTemplate: Array<CmsContentBlock>
    initialState?: cprud
    text: {
        headerReadBody?: string
        headerPreviewBody?: string
        headerCreateHeading?: string
        headerCreateBody?: string
        headerUpdateHeading?: string
        headerUpdateBody?: string
        headerEnterUpdateButton?: string
        headerLeaveUpdateButton?: string
        headerDiscardUpdateButton?: string
        headerPreviewUpdateButton?: string
        headerPublishUpdateButton?: string
        createButton: string
        cancelCreateButton: string
    }
    shouldRenderEditingHeader?: boolean
    discardUpdateAction?: () => void
    stateChangeAction?: (currentState: cprud) => void
    createBlockAction?: (
        blockTypeContentId: string,
        parentBlockId?: string
    ) => Promise<string>
    changeBlocksAction?: (blocks: Array<CmsContentBlock>) => void
    publishBlocksAction?: (
        blocks: Array<CmsContentBlock>,
        localErrors: FormErrors
    ) => Promise<FormErrors>
    themeId: string
    className?: string
}
