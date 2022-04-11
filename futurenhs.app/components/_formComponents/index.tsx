import { Input } from './Input';
import { ImageUpload } from './ImageUpload';
import { TextArea } from './TextArea';
import { Hidden } from './Hidden';
import { MultiChoice } from './MultiChoice';
import { CheckBox } from './CheckBox';
import { FieldSet } from './FieldSet';
import { GroupContainer } from './GroupContainer';
import { AutoComplete } from './AutoComplete';

export const formComponents: any = {
    input: Input,
    imageUpload: ImageUpload,
    textArea: TextArea,
    fieldSet: FieldSet,
    multiChoice: MultiChoice,
    checkBox: CheckBox,
    hidden: Hidden,
    groupContainer: GroupContainer,
    autoComplete: AutoComplete
};
