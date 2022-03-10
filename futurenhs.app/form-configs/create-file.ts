import { formTypes } from "@constants/forms";
import { FormConfig } from '@appTypes/form';

export const createFileForm: FormConfig = {
    id: formTypes.CREATE_FILE,
    steps: [
        {
            fields: [
                {
                    name: 'File',
                    text: {
                        label: 'File'
                    },
                    component: 'input',
                    inputType: 'file',
                    validators: [
                        {
                            type: 'required',
                            message: 'Add a file'
                        }
                    ]
                },
                {
                    name: 'FieldWrapper',
                    component: 'groupContainer',
                    className: 'u-mb-14 u-p-4 tablet:u-px-14 tablet:u-pt-12 u-pb-8 u-bg-theme-1',
                    fields: [
                        {
                            name: 'Title',
                            inputType: 'text',
                            text: {
                                label: 'Enter a file title',
                            },
                            component: 'input',
                            validators: [
                                {
                                    type: 'required',
                                    message: 'Enter the file title'
                                }
                            ]
                        },
                        {
                            name: 'Description',
                            text: {
                                label: 'Enter a file description (optional)'
                            },
                            component: 'textArea'
                        }
                    ]
                }
            ]
        }
    ]
};