import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch'
import { ServiceError } from '..'
import { FetchResponse } from '@appTypes/fetch'
import { ApiResponse, ServiceResponse } from '@appTypes/service'
import { Folder } from '@appTypes/file'
import { User } from '@appTypes/user'

declare type Options = {
    user: User
    groupId: string
    folderId: string
    isForUpdate?: boolean
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const getGroupFolder = async (
    { user, groupId, folderId, isForUpdate }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<Folder>> => {
    const serviceResponse: ServiceResponse<Folder> = {
        data: null,
    }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const id: string = user.id

    const apiUrl: string = `${
        process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL
    }/v1/users/${id}/groups/${groupId}/folders/${folderId}${
        isForUpdate ? '/update' : ''
    }`
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
            'An unexpected error occurred when attempting to get the group folder',
            {
                serviceId: services.GET_GROUP_FOLDER,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    const reversedPath: Array<any> = apiData.path?.reverse() ?? []

    serviceResponse.headers = headers
    serviceResponse.data = {
        id: apiData.id,
        type: 'folder',
        text: {
            name: apiData.name,
            body: apiData.description,
        },
        path:
            reversedPath.map(({ id, name }) => ({
                element: id,
                text: name,
            })) ?? [],
    }

    return serviceResponse
}
