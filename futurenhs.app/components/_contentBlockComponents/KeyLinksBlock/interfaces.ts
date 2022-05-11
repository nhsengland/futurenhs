import { CmsContentBlock } from "@appTypes/contentBlock";
import { FormErrors } from "@appTypes/form";

export interface Props {
    id?: string
    isEditable?: boolean;
    block: CmsContentBlock;
    headingLevel: number;
    themeId?: string;
    initialErrors?: FormErrors;
    maxLinks?: number;
    createAction: any;
    changeAction: (config: {
        block: CmsContentBlock;
        errors: FormErrors;
    }) => void
    className?: string
}
