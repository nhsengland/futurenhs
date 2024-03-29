import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { requestMethods, defaultTimeOutMillis } from '@constants/fetch'
import { ServiceError } from '..'
import { FetchResponse } from '@appTypes/fetch'
import { ApiResponse, ServiceResponse } from '@appTypes/service'
import { User } from '@appTypes/user'
import { CmsContentBlock } from '@appTypes/contentBlock'
import jwtHeader from '@helpers/util/jwt/jwtHeader'

declare type Options = {
    user: User
    pageId: string
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const postCmsPageContent = async (
    { user, pageId }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<any>> => {
    const serviceResponse: ServiceResponse<Array<CmsContentBlock>> = {
        data: null,
    }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const id: string = user.id

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/page/${pageId}`
    const authHeader = jwtHeader(user.accessToken)
    const apiHeaders = setFetchOptions({
        method: requestMethods.POST,
        headers: authHeader,
    })
    const apiResponse: FetchResponse = await fetchJSON(
        apiUrl,
        apiHeaders,
        defaultTimeOutMillis
    )

    const apiData: ApiResponse<any> = apiResponse.json
    const apiMeta: any = apiResponse.meta

    const { headers, ok, status, statusText } = apiMeta

    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to publish the cms page content',
            {
                serviceId: services.POST_CMS_PAGE_CONTENT,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    serviceResponse.headers = headers
    serviceResponse.data = apiData?.data?.content?.pageContent?.blocks

    return serviceResponse
}
