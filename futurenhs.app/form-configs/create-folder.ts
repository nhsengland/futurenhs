import { formTypes } from "@constants/forms";
import { FormConfig } from '@appTypes/form';

export const createFolderForm: FormConfig = {
    id: formTypes.CREATE_FOLDER,
    steps: [
        {
            fields: [
                {
                    name: 'name',
                    inputType: 'text',
                    text: {
                        label: 'Enter a folder title',
                        hint: 'This is a hint offering some short guidance to users'
                    },
                    component: 'input',
                    shouldRenderRemainingCharacterCount: true,
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter the folder title'
                        },
                        {
                            type: 'maxLength',
                            maxLength: 1000,
                            message: 'Enter 1000 or fewer characters'
                        }
                    ]
                },
                {
                    name: 'description',
                    text: {
                        label: 'Enter a folder description',
                        hint: 'This is a hint offering some short guidance to users'
                    },
                    component: 'textArea',
                    shouldRenderRemainingCharacterCount: true,
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter the folder description'
                        },
                        {
                            type: 'maxLength',
                            maxLength: 4000,
                            message: 'Enter 4000 or fewer characters'
                        }
                    ]
                }
            ]
        }
    ]
};