import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { requestMethods, defaultTimeOutMillis } from '@constants/fetch'
import { ServiceError } from '..'
import { FetchResponse } from '@appTypes/fetch'
import { ApiResponse, ServiceResponse } from '@appTypes/service'
import { Group } from '@appTypes/group'
import { User } from '@appTypes/user'

declare type Options = {
    user: User
    groupId: string
    isForUpdate?: boolean
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export type GetGroupService = (
    options: Options,
    dependencies?: Dependencies
) => Promise<ServiceResponse<Group>>

export const getGroup = async (
    { user, groupId, isForUpdate }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<Group>> => {
    const serviceResponse: ServiceResponse<Group> = {
        data: null,
    }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const id: string = user.id

    const apiUrl: string = `${
        process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL
    }/v1/users/${id}/groups/${groupId}${isForUpdate ? '/update' : ''}`

    const apiResponse: FetchResponse = await fetchJSON(
        apiUrl,
        setFetchOptions({ method: requestMethods.GET }),
        defaultTimeOutMillis
    )

    const apiData: ApiResponse<any> = apiResponse.json
    const apiMeta: any = apiResponse.meta

    const { headers, ok, status, statusText } = apiMeta

    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to get the group',
            {
                serviceId: services.GET_GROUP,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    serviceResponse.headers = headers
    serviceResponse.data = {
        text: {
            title: apiData.name ?? null,
            metaDescription: 'A FutureNHS group',
            mainHeading: apiData.name ?? null,
            strapLine: apiData?.strapline ?? null,
        },
        image: apiData.image
            ? {
                  src: `${apiData.image?.source}`,
                  height: apiData?.image?.height ?? null,
                  width: apiData?.image?.width ?? null,
                  altText: 'Group logo',
              }
            : null,
        imageId: apiData.imageId,
        themeId: apiData.themeId,
        isPublic: apiData.isPublic
    }

    return serviceResponse
}
