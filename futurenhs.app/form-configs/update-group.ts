import { formTypes } from "@constants/forms";
import { themes } from "@constants/themes";
import { FormConfig } from '@appTypes/form';

export const updateGroupForm: FormConfig = {
    id: formTypes.UPDATE_GROUP,
    steps: [
        {
            fields: [
                {
                    name: 'name',
                    inputType: 'text',
                    text: {
                        label: 'Group name',
                        hint: 'Add your group name, this will be used to search for the group '
                    },
                    component: 'input',
                    shouldRenderRemainingCharacterCount: true,
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter the group name'
                        },
                        {
                            type: 'maxLength',
                            maxLength: 450,
                            message: 'Enter 450 or fewer characters'
                        }
                    ]
                },
                {
                    name: 'strapline',
                    text: {
                        label: 'Strap line (optional)',
                        hint: 'Add a strapline to encapsulate your group, include keywords for search'
                    },
                    component: 'textArea',
                    shouldRenderRemainingCharacterCount: true,
                    validators: [
                        {
                            type: 'maxLength',
                            maxLength: 254,
                            message: 'Enter 254 or fewer characters'
                        }
                    ]
                },
                {
                    name: 'logo',
                    text: {
                        label: 'Logo (optional)',
                        hint: 'Please upload your logo or an icon. If not, we will use our default FutureNHS icon shown.'
                    },
                    component: 'input',
                    inputType: 'file'
                },
                {
                    name: 'theme',
                    text: {
                        label: 'Choose your theme colour',
                        hint: 'Please choose a colour theme for your group. Please note, all colour combinations are accessible. For more information, see our knowledge hub.'
                    },
                    component: 'multiChoice',
                    options: [
                        {
                            name: '36d49305-eca8-4176-bfea-d25af21469b9',
                            label: 'Theme 1'
                        },
                        {
                            name: '0fca6809-a71f-4733-a622-343967acbad9',
                            label: 'Theme 2'
                        },
                        {
                            name: '9a3c911b-c3d3-4f58-a32a-d541e0f5bf56',
                            label: 'Theme 3'
                        },
                        {
                            name: '5e7b315b-67b0-457d-8d44-ed2d4bcfac1d',
                            label: 'Theme 4'
                        },
                        {
                            name: '5053a8c6-ea4d-4125-9dc3-475e3e931fee',
                            label: 'Theme 5'
                        },
                        {
                            name: '53bce171-d6a3-4721-8199-92f10fca5ef2',
                            label: 'Theme 6'
                        },
                        {
                            name: 'e71b38fc-c200-4a7c-b484-6bc786fd0aa6',
                            label: 'Theme 7'
                        },
                        {
                            name: '217a1f99-5b25-4e3b-be3d-29c55c46be05',
                            label: 'Theme 8'
                        },
                        {
                            name: 'a7101d5f-acce-4ef7-b1c9-5dbf20d54580',
                            label: 'Theme 9'
                        },
                        {
                            name: 'a9d8566d-162a-4fa3-b159-f604b12c214d',
                            label: 'Theme 10'
                        }
                    ],
                    validators: [
                        {
                            type: 'required',
                            message: 'Select the group theme'
                        }
                    ],
                    optionClassName: 'tablet:u-w-2/5'
                },
                {
                    name: 'features',
                    text: {
                        legend: 'Features included in your group',
                        hint: 'Choose to include a forum, a file area and a members list on your group. These will be displayed in the group header.'
                    },
                    component: 'fieldSet',
                    fields: [
                        {
                            name: 'include-forum-page',
                            inputType: 'multiChoice',
                            text: {
                                label: 'Include a forum page?'
                            },
                            options: [
                                {
                                    name: 'yes',
                                    label: 'Yes'
                                },
                                {
                                    name: 'no',
                                    label: 'No'
                                }
                            ],
                            component: 'multiChoice',
                            validators: [
                                {
                                    type: 'required',
                                    message: 'Select whether the group should include a forum page'
                                }
                            ]
                        },
                        {
                            name: 'include-files-page',
                            inputType: 'text',
                            text: {
                                label: 'Include a files page?'
                            },
                            options: [
                                {
                                    name: 'yes',
                                    label: 'Yes'
                                },
                                {
                                    name: 'no',
                                    label: 'No'
                                }
                            ],
                            component: 'multiChoice',
                            validators: [
                                {
                                    type: 'required',
                                    message: 'Select whether the group should include a files page'
                                }
                            ]
                        },
                        {
                            name: 'include-members-page',
                            inputType: 'multiChoice',
                            text: {
                                label: 'Include a members page?'
                            },
                            options: [
                                {
                                    name: 'yes',
                                    label: 'Yes'
                                },
                                {
                                    name: 'no',
                                    label: 'No'
                                }
                            ],
                            component: 'multiChoice',
                            validators: [
                                {
                                    type: 'required',
                                    message: 'Select whether the group should include a members page'
                                }
                            ]
                        }
                    ]
                }
            ]
        }
    ]
};