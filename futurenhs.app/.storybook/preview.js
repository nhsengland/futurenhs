import '../assets/scss/screen.scss'
import * as NextImage from 'next/image'

import { themes } from '../constants/themes'
import formConfigs from '@formConfigs/index'
import { ThemesContext, FormsContext, LoadingContext } from '../contexts/index'

const OriginalNextImage = NextImage.default

Object.defineProperty(NextImage, 'default', {
    configurable: true,
    value: (props) => <OriginalNextImage {...props} unoptimized />,
})

const formsContextConfig = {
    csrfToken: 'mockCsrfToken',
    templates: formConfigs,
}

const themesContextConfig = {
    themes,
}

const loadingContextConfig = {
    isLoading: true,
    text: {
        loadingMessage: '',
    },
}

export const parameters = {
    actions: { argTypesRegex: '^on[A-Z].*' },
    controls: {
        matchers: {
            color: /(background|color)$/i,
            date: /Date$/,
        },
    },
    previewTabs: {
        'storybook/docs/panel': { index: -1 },
    },
    viewMode: 'docs',
}

export const decorators = [
    (Story) => (
        <FormsContext.Provider value={formsContextConfig}>
            <LoadingContext.Provider value={loadingContextConfig}>
                <ThemesContext.Provider value={themesContextConfig}>
                    <Story />
                </ThemesContext.Provider>
            </LoadingContext.Provider>
        </FormsContext.Provider>
    ),
]
