import { Input } from './index'
import { Form } from '@components/Form'

export default {
    title: 'Input',
    component: Input,
    argTypes: {
        input: {
            control: { type: '' },
        },
        meta: {
            control: { type: '' },
        },
        initialError: {
            control: { type: '' },
        },
        validators: {
            control: { type: '' },
        },
    },
}

const Template = (args) => (
    <Input input={{}} meta={{}} {...args} className="u-w-2/3" />
)
const CharacterCountTemplate = (args) => (
    <Form
        csrfToken="123"
        shouldRenderSubmitButton={false}
        {...args}
        className="u-w-2/3"
    />
)

export const Basic = Template.bind({})
Basic.args = {
    text: {
        label: 'Example input field',
    },
}

export const Hint = Template.bind({})
Hint.args = {
    text: {
        label: 'Example input field',
        hint: 'Hint for input field',
    },
}

export const CharacterCount = CharacterCountTemplate.bind({})
CharacterCount.args = {
    formConfig: {
        id: '1',
        steps: [
            {
                fields: [
                    {
                        name: 'Input',
                        inputType: 'text',
                        text: {
                            label: 'Example input field',
                        },
                        shouldRenderRemainingCharacterCount: true,
                        component: 'input',
                        validators: [
                            {
                                type: 'maxLength',
                                maxLength: 30,
                                message: 'Enter 30 or fewer characters',
                            },
                        ],
                    },
                ],
            },
        ],
    },
}
