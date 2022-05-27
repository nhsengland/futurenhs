import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { requestMethods, defaultTimeOutMillis } from '@constants/fetch'
import { ServiceError } from '..'
import { ServiceResponse } from '@appTypes/service'
import { User } from '@appTypes/user'

declare type Options = {
    user: User
    blockContentTypeId: string
    pageId?: string
    parentBlockId?: string
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const postCmsBlock = async (
    { blockContentTypeId, pageId, parentBlockId, user }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<string>> => {
    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const { id } = user

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/block/${id}`
    const apiResponse: any = await fetchJSON(
        apiUrl,
        setFetchOptions({
            method: requestMethods.POST,
            body: {
                contentType: blockContentTypeId,
                parentId: parentBlockId ?? pageId,
            },
        }),
        defaultTimeOutMillis
    )

    const apiMeta: any = apiResponse.meta
    const apiData: any = apiResponse.json

    const { ok, status, statusText } = apiMeta

    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to post the new CMS block',
            {
                serviceId: services.POST_CMS_BLOCK,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    return apiData
}
