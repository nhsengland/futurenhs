import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch'
import { ServiceError } from '..'
import { ServiceResponse } from '@appTypes/service'
import { api } from '@constants/routes'

declare type Options = {
    slug: string
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const getFeatureEnabled = async (
    { slug },
    dependencies?: Dependencies
): Promise<ServiceResponse<boolean>> => {
    const serviceResponse: ServiceResponse<boolean> = {
        data: null,
    }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper
    const apiUrl: string = `${
        process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL
    }${api.FEATURE_FLAG.replace('%FLAG%', slug)}`
    const apiHeaders = setFetchOptions({
        method: requestMethods.GET,
    })

    const apiResponse: any = await fetchJSON(
        apiUrl,
        apiHeaders,
        defaultTimeOutMillis
    )

    const apiData: any = apiResponse.json
    const apiMeta: any = apiResponse.meta

    const { ok, status, statusText } = apiMeta
    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to get site feature flags',
            {
                serviceId: services.GET_FEATURE_ENABLED,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }
    const data = apiData

    return {
        data: data,
    }
}
