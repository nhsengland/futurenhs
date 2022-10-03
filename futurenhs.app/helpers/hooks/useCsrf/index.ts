import { useContext } from 'react'

import { FormsContext } from '@helpers/contexts/index'

export const useCsrf = (): string => {
    const config: any = useContext(FormsContext)

    return config.csrfToken
}
