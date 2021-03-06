import { useContext } from 'react'

import { FormsContext } from '@contexts/index'

export const useCsrf = (): string => {
    const config: any = useContext(FormsContext)

    return config.csrfToken
}
