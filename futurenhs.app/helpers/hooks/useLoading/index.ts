import { useContext } from 'react'

import { LoadingContext } from '@helpers/contexts/index'

export const useLoading = (): {
    isLoading: boolean
    text: {
        loadingMessage: string
    }
} => {
    const config: any = useContext(LoadingContext)

    return config
}
