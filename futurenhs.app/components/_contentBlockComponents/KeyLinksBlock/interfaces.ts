import { CmsContentBlock } from "@appTypes/contentBlock";
import { FormErrors } from "@appTypes/form";

export interface Props {
    id?: string
    isEditable?: boolean;
    block: CmsContentBlock;
    headingLevel: number;
    themeId?: string;
    initialErrors?: FormErrors;
    changeAction: (config: {
        instanceId: string;
        formState: Record<any, any>;
    }) => void
    className?: string
}