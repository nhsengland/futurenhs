import { FormConfig } from '@appTypes/form'
import { formTypes } from '@constants/forms'
import { useFormConfig } from '@hooks/useForm'
import { Form } from './index'

export default {
    title: 'Form',
    component: Form
}

const formConfig: FormConfig = {
    id: 'testId',
    steps: [
        {
            fields: [
                {
                    name: 'image',
                    text: {
                        label: 'Upload a profile picture',
                        hint: 'The selected file must be a JPG or PNG and must be smaller than 5MB.',
                    },
                    component: 'imageUpload',
                    relatedFields: {
                        fileId: 'ImageId',
                    },
                },
                {
                    name: 'ImageId',
                    component: 'hidden',
                },
                {
                    name: 'firstName',
                    inputType: 'text',
                    text: {
                        label: 'First name',
                    },
                    component: 'input',
                    shouldRenderRemainingCharacterCount: true,
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter a name',
                        },
                        {
                            type: 'maxLength',
                            maxLength: 255,
                            message: 'Enter 255 or fewer characters',
                        },
                    ],
                },
                {
                    name: 'lastName',
                    inputType: 'text',
                    text: {
                        label: 'Last name (optional)',
                    },
                    component: 'input',
                    shouldRenderRemainingCharacterCount: true,
                    validators: [
                        {
                            type: 'maxLength',
                            maxLength: 255,
                            message: 'Enter 255 or fewer characters',
                        },
                    ],
                },
                {
                    name: 'pronouns',
                    inputType: 'text',
                    text: {
                        label: 'Preferred pronouns (optional)',
                        hint: 'Example: she/her, he/him, they/them',
                    },
                    component: 'input',
                    shouldRenderRemainingCharacterCount: true,
                    validators: [
                        {
                            type: 'maxLength',
                            maxLength: 255,
                            message: 'Enter 255 or fewer characters',
                        },
                    ],
                },
                {
                    name: 'Bio',
                    component: 'textArea',
                    shouldRenderAsRte: true,
                    text: {
                        label: 'Bio',
                        hint: 'Tell us about yourself'
                    }
                },
                {
                    name: 'Notifications',
                    text: {
                        label: 'Would you like to receive notifications?',
                    },
                    component: 'multiChoice',
                    options: [
                        {
                            value: 'Yes',
                            label: 'Yes'
                        },
                        {
                            value: 'No',
                            label: 'No'
                        },
                    ]
                },
                {
                    name: 'terms',
                    component: 'checkBox',
                    inputType: 'checkbox',
                    text: {
                        label: 'Please confirm that all changes are in line with the platforms <a href="/terms-and-conditions">terms and conditions</a>',
                    },
                    validators: [
                        {
                            type: 'required',
                            message:
                                'Select to confirm the terms and conditions',
                        },
                    ],
                },
            ],
        },
    ],
}

const Template = (args) =>
    <div className="tablet:u-w-2/3">
        <h2>Edit your profile</h2>
        <Form
            submitAction={() => true}
            cancelAction={() => true}
            csrfToken={'1234'}
            text={{
                submitButton: 'Submit',
                cancelButton: 'Discard'
            }}

            {...args}
        />
    </div>





export const Basic = Template.bind({})
Basic.args = {
    formConfig: formConfig
}