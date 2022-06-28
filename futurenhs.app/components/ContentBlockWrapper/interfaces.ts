import { CmsContentBlock } from '@appTypes/cmsContent'
import { cprud } from '@constants/cprud'

export interface Props {
    mode: cprud
    block: CmsContentBlock
    isInEditMode?: boolean
    shouldRenderMovePrevious?: boolean
    shouldRenderMoveNext?: boolean
    shouldEnableMovePrevious?: boolean
    shouldEnableMoveNext?: boolean
    shouldEnableEnterUpdate?: boolean
    shouldEnableEnterRead?: boolean
    shouldEnableDelete?: boolean
    createAction?: (blockId: string | number) => void
    deleteAction?: (blockId: string | number) => void
    movePreviousAction?: (blockId: string | number) => void
    moveNextAction?: (blockId: string | number) => void
    enterUpdateModeAction?: (blockId: string | number) => void
    enterReadModeAction?: (blockId: string | number) => void
    children: any
    className?: string
}
