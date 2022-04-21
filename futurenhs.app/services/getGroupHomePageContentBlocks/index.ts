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
import { ContentBlock } from '@appTypes/contentBlock';

declare type Options = {
    user: User
    groupId: string
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export type getGroupHomePageContentService = (
    options: Options,
    dependencies?: Dependencies
) => Promise<ServiceResponse<Array<ContentBlock>>>

export const getGroupHomePageContentBlocks = async (
    { user, groupId }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<any>> => {
    const serviceResponse: ServiceResponse<Array<ContentBlock>> = {
        data: null,
    }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const id: string = user.id

    const contentSiteMapApiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/groups/${groupId}/site`

    const contentSiteMapApiResponse: FetchResponse = await fetchJSON(
        contentSiteMapApiUrl,
        setFetchOptions({ method: requestMethods.GET }),
        defaultTimeOutMillis
    )

    const contentSiteMapApiData: ApiResponse<any> = contentSiteMapApiResponse.json
    const contentSiteMapApiMeta: any = contentSiteMapApiResponse.meta

    const { headers, ok, status, statusText } = contentSiteMapApiMeta

    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to get the group homepage content',
            {
                serviceId: services.GET_GROUP_HOME_PAGE_CONTENT,
                status: status,
                statusText: statusText,
                body: contentSiteMapApiData,
            }
        )
    }

    const pageContentApiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/page/${contentSiteMapApiData.contentRootId}`

    const pageContentApiResponse: FetchResponse = await fetchJSON(
        pageContentApiUrl,
        setFetchOptions({ method: requestMethods.GET }),
        defaultTimeOutMillis
    )

    const pageContentApiData: ApiResponse<any> = pageContentApiResponse.json

    serviceResponse.headers = headers
    serviceResponse.data = pageContentApiData?.payload?.fields?.pageContent?.fields.map((field) => {

        console.log(field);

        return {
            instanceId: field.system.id,
            typeId: field.system.contentType,
            fields: field.fields         
        } as ContentBlock

    });

    return serviceResponse
}
