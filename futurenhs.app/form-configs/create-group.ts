import { formTypes } from '@constants/forms'
import { themes } from '@constants/themes'
import { services as serviceIds } from '@constants/services'
import { selectTheme } from '@selectors/themes'
import { isLight } from '@helpers/util/theme/isLight'
import { FormConfig } from '@appTypes/form'
import { Theme } from '@appTypes/theme'

const getThemeLabel = ({ themeId, number }): string => {
    const { background, content, accent }: Theme = selectTheme(themes, themeId)

    const backgroundBorderClass: string = isLight({ colorId: background })
        ? 'u-border-theme-4'
        : `u-border-theme-${background}`
    const contentBorderClass: string = isLight({ colorId: content })
        ? 'u-border-theme-4'
        : `u-border-theme-${content}`
    const accentBorderClass: string = isLight({ colorId: accent })
        ? 'u-border-theme-4'
        : `u-border-theme-${accent}`

    return `<span class="c-theme-tokens">
                <span class=\"u-sr-only\">Theme ${number}</span>
                <span label="Background" class="c-theme-tokens_theme u-w-10 u-h-10 u-bg-theme-${background} ${backgroundBorderClass}"></span>
                <span label="Text" class="c-theme-tokens_theme u-w-6 u-h-6 u-bg-theme-${content} ${contentBorderClass}"></span>
                <span label="Accent" class="c-theme-tokens_theme u-w-6 u-h-6 u-bg-theme-${accent} ${accentBorderClass}"></span>
            </span>`
}

export const createGroupForm: FormConfig = {
    id: formTypes.CREATE_GROUP,
    steps: [
        {
            fields: [
                {
                    name: 'name',
                    inputType: 'text',
                    text: {
                        label: 'Group name',
                        hint: 'Add your group name, this will be used to search for the group ',
                    },
                    component: 'input',
                    shouldRenderRemainingCharacterCount: true,
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter the group name',
                        },
                        {
                            type: 'maxLength',
                            maxLength: 255,
                            message: 'Enter 255 or fewer characters',
                        },
                    ],
                },
                {
                    name: 'strapline',
                    text: {
                        label: 'Strap line (optional)',
                        hint: 'Add a strapline to encapsulate your group, include keywords for search',
                    },
                    component: 'input',
                    inputType: 'text',
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
                    name: 'image',
                    text: {
                        label: 'Logo (optional)',
                        hint: 'Please upload your logo or an icon. If not, we will use the default image. The selected file must be a JPG or PNG and must be smaller than 5MB.',
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
                    name: 'themeId',
                    text: {
                        label: 'Choose your theme colour',
                        hint: 'Please choose a colour theme for your group. Please note, all colour combinations are accessible. For more information, see our knowledge hub.',
                    },
                    component: 'multiChoice',
                    options: [
                        {
                            value: '36d49305-eca8-4176-bfea-d25af21469b9',
                            label: getThemeLabel({
                                themeId: '36d49305-eca8-4176-bfea-d25af21469b9',
                                number: 1,
                            }),
                        },
                        {
                            value: '9a3c911b-c3d3-4f58-a32a-d541e0f5bf56',
                            label: getThemeLabel({
                                themeId: '9a3c911b-c3d3-4f58-a32a-d541e0f5bf56',
                                number: 3,
                            }),
                        },
                        {
                            value: '5053a8c6-ea4d-4125-9dc3-475e3e931fee',
                            label: getThemeLabel({
                                themeId: '5053a8c6-ea4d-4125-9dc3-475e3e931fee',
                                number: 5,
                            }),
                        },
                    ],
                    validators: [
                        {
                            type: 'required',
                            message: 'Select the group theme',
                        },
                    ],
                    optionClassName: 'tablet:u-w-2/5',
                },
                {
                    name: 'groupOwnerId',
                    text: {
                        label: 'Group owner',
                    },
                    component: 'autoComplete',
                    shouldPreventFreeText: true,
                    shouldRenderRemainingCharacterCount: false,
                    serviceId: serviceIds.GET_SITE_USERS_BY_TERM,
                    validators: [
                        {
                            type: 'required',
                            message: 'Enter a valid group owner',
                        },
                        {
                            type: 'maxLength',
                            maxLength: 255,
                            message: 'Enter 1000 or fewer characters',
                        },
                    ],
                },
                {
                    name: 'groupAdminId',
                    text: {
                        label: 'Group administrators (optional)',
                    },
                    component: 'autoComplete',
                    shouldPreventFreeText: true,
                    shouldRenderRemainingCharacterCount: false,
                    serviceId: serviceIds.GET_SITE_USERS_BY_TERM,
                    validators: [
                        {
                            type: 'maxLength',
                            maxLength: 255,
                            message: 'Enter 1000 or fewer characters',
                        },
                    ],
                },
                {
                    name: 'public',
                    component: 'checkBox',
                    inputType: 'checkbox',
                    text: {
                        label: 'Group is public?',
                    },
                },
            ],
        },
    ],
}
