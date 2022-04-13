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
                    },
                    component: 'imageUpload',
                    relatedFields: {
                        fileId: 'ImageId'
                    }
                },
                {
                    name: 'ImageId',
                    component: 'hidden'
                },
                {
                    name: 'firstName',
                    inputType: 'text',
                    text: {
                        label: 'First name',
                    },
                    component: 'input',
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter a name',
                        },
                        {
                            type: 'maxLength',
                            maxLength: 255,
                            message: 'Enter 255 or fewer characters'
                        }
                    ],
                },
                {
                    name: 'lastName',
                    inputType: 'text',
                    text: {
                        label: 'Last name (optional)',
                    },
                    component: 'input',
                    validators: [
                        {
                            type: 'maxLength',
                            maxLength: 255,
                            message: 'Enter 255 or fewer characters'
                        }
                    ]
                },
                {
                    name: 'pronouns',
                    inputType: 'text',
                    text: {
                        label: 'Preferred pronouns (optional)',
                        hint: 'Example: she/her, he/him, they/them'
                    },
                    component: 'input',
                    validators: [
                        {
                            type: 'maxLength',
                            maxLength: 255,
                            message: 'Enter 255 or fewer characters'
                        }
                    ]
                },
                {
                    name: 'terms',
                    component: 'checkBox',
                    text: {
                        label: 'Please confirm that all changes are in line with the platforms <a href="/terms-and-conditions">terms and conditions</a>'
                    },
                    validators: [
                        {
                            type: 'required',
                            message: 'Select to confirm the terms and conditions',
                        },
                    ]
                }
            ],
        },
    ],
}