import { CmsContentBlock } from "@appTypes/contentBlock";
import { FormErrors } from "@appTypes/form";

export interface Props {
    id?: string
    block: CmsContentBlock
    isEditable?: boolean;
    headingLevel: number;
    changeAction: (config: {
        instanceId: string;
        formState: Record<any, any>;
    }) => void
    initialErrors?: FormErrors;
    className?: string
}
