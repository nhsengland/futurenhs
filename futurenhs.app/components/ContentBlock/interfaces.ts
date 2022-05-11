import { FormErrors } from "@appTypes/form";
import { CmsContentBlock } from "@appTypes/contentBlock";
import { cprud } from '@constants/cprud'

export interface Props {
    typeId: string;
    instanceId: string;
    themeId: string;
    mode: cprud;
    block: CmsContentBlock;
    isEditable?: boolean;
    isInEditMode?: boolean;
    shouldRenderMovePrevious?: boolean;
    shouldRenderMoveNext?: boolean;
    text: {
        name: string;
    };
    changeAction: (config: {
        instanceId: string;
        block: CmsContentBlock;
        errors: FormErrors;
    }) => void
    initialErrors?: FormErrors;
    createAction?: (id: string | number) => void;
    deleteAction?: (id: string | number) => void;
    movePreviousAction?: (id: string | number) => void;
    moveNextAction?: (id: string | number) => void;
    enterUpdateModeAction?: (id: string | number) => void;
    enterReadModeAction?: (id: string | number) => void;
    className?: string
}
