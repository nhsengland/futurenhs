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
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter the folder title'
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
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter the folder description'
                        }
                    ]
                }
            ]
        }
    ]
};