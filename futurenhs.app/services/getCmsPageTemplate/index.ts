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

declare type Options = {
    user: User
    templateId: string
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const getCmsPageTemplate = async (
    { user, templateId }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<any>> => {
    const serviceResponse: ServiceResponse<Array<CmsContentBlock>> = {
        data: null,
    }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const id: string = user.id

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/template`

    const contentTemplatesApiResponse: FetchResponse = await fetchJSON(
        apiUrl,
        setFetchOptions({ method: requestMethods.GET }),
        defaultTimeOutMillis
    )

    const apiData: ApiResponse<any> = contentTemplatesApiResponse.json
    const apiMeta: any = contentTemplatesApiResponse.meta

    const { headers, ok, status, statusText } = apiMeta

    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to get the cms page template',
            {
                serviceId: services.GET_CMS_PAGE_TEMPLATE,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    const template = apiData.data?.find(
        (template) => template.item.id === templateId
    )

    serviceResponse.headers = headers
    serviceResponse.data = template.content.blocks

    return serviceResponse
}
