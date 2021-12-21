import { Form } from '@appTypes/form';

export const createFolderForm: Form = {
    steps: [
        {
            fields: [
                {
                    name: 'name',
                    inputType: 'text',
                    content: {
                        labelText: 'Enter a folder title',
                        hintHtml: 'This is a hint offering some short guidance to users'
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
                    content: {
                        labelText: 'Enter a folder description',
                        hintHtml: 'This is a hint offering some short guidance to users'
                    },
                    component: 'textArea'
                }
            ]
        }
    ]
};