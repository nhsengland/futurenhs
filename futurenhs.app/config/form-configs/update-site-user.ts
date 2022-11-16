import { formTypes } from '@constants/forms'
import { FormConfig } from '@appTypes/form'

export const updateSiteMemberForm: FormConfig = {
    id: formTypes.UPDATE_SITE_USER,
    steps: [
        {
            fields: [
                {
                    name: 'image',
                    text: {
                        label: 'Image',
                        hint: 'The selected file must be a JPG or PNG and must be smaller than 5MB.',
                    },
                    component: 'imageUpload',
                    relatedFields: {
                        fileId: 'imageId',
                    },
                },
                {
                    name: 'imageId',
                    component: 'hidden',
                },
                {
                    name: 'firstName',
                    inputType: 'text',
                    text: {
                        label: 'First name',
                    },
                    component: 'input',
                    autoComplete: 'given-name',
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
                    autoComplete: 'family-name',
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
                    name: 'email',
                    inputType: 'email',
                    disabled: true,
                    text: {
                        label: 'Email address',
                        hint: 'To change your email address please contact <a href="https://support-futurenhs.zendesk.com/hc/en-gb/articles/7615088162973">Zendesk</a>',
                    },
                    component: 'input',
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
