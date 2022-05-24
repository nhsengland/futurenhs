import { CmsContentBlock } from "@appTypes/contentBlock";
import { FormErrors } from "@appTypes/form";

export interface Props {
    id?: string
    block: CmsContentBlock
    isEditable?: boolean;
    headingLevel: number;
    changeAction: (config: {
        block: CmsContentBlock;
        errors?: FormErrors;
    }) => void
    initialErrors?: Record<string, FormErrors>;
    className?: string
}
