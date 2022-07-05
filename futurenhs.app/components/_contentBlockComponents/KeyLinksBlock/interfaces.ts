import { CmsContentBlock } from '@appTypes/cmsContent'
import { FormErrors } from '@appTypes/form'

export interface Props {
    id?: string
    isEditable?: boolean
    isPreview?: boolean
    block: CmsContentBlock
    headingLevel: number
    themeId?: string
    initialErrors?: Record<string, FormErrors>
    maxLinks?: number
    createAction: any
    changeAction: (config: {
        block: CmsContentBlock
        errors?: FormErrors
        childBlockId?: string
    }) => void
    className?: string
}
