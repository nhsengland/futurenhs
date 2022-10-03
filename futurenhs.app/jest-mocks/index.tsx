import React from 'react'
import { render } from '@testing-library/react'

import { ThemesContext, FormsContext } from '@helpers/contexts/index'
import formConfigs from '@config/form-configs/index'
import { themes } from '@constants/themes'

const formsContextConfig: Record<string, any> = {
    csrfToken: 'mockCsrfToken',
    templates: formConfigs,
}

const themesContextConfig: Record<string, any> = {
    themes: themes,
}

const Providers = ({ children }) => {
    return (
        <ThemesContext.Provider value={themesContextConfig}>
            <FormsContext.Provider value={formsContextConfig}>
                {children}
            </FormsContext.Provider>
        </ThemesContext.Provider>
    )
}

const customRender = (ui, options?: any) =>
    render(ui, { wrapper: Providers, ...options })

// re-export testing-library
export * from '@testing-library/react'

// override render method
export { customRender as render }
