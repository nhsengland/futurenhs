import { CmsContentBlock } from '@appTypes/cmsContent'
import { FormErrors } from '@appTypes/form'

export interface Props {
    id?: string
    block: CmsContentBlock
    isEditable?: boolean
    headingLevel: number
    changeAction: (config: {
        block: CmsContentBlock
        errors?: FormErrors
    }) => void
    initialErrors?: Record<string, FormErrors>
    className?: string
}
