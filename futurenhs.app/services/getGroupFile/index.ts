import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch'
import { ServiceError } from '..'
import { FetchResponse } from '@appTypes/fetch'
import { ApiResponse, ServiceResponse } from '@appTypes/service'
import { FolderContent } from '@appTypes/file'
import { User } from '@appTypes/user'

declare type Options = {
    user: User
    groupId: string
    fileId: string
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const getGroupFile = async (
    { user, groupId, fileId }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<FolderContent>> => {
    const serviceResponse: ServiceResponse<FolderContent> = {
        data: null,
    }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const id: string = user.id

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/users/${id}/groups/${groupId}/files/${fileId}`
    const apiResponse: FetchResponse = await fetchJSON(
        apiUrl,
        setFetchOptions({ method: requestMethods.GET }),
        defaultTimeOutMillis
    )

    const apiData: ApiResponse<any> = apiResponse.json
    const apiMeta: any = apiResponse.meta

    const { ok, status, statusText } = apiMeta

    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to get the group file',
            {
                serviceId: services.GET_GROUP_FILE,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    const reversedPath: Array<any> = apiData.path?.reverse() ?? []

    serviceResponse.data = {
        id: apiData.id,
        type: 'file',
        name: apiData.name,
        text: {
            body: apiData.description,
        },
        created: apiData.firstRegistered?.atUtc,
        createdBy: {
            id: apiData.firstRegistered?.by?.id,
            text: {
                userName: apiData.firstRegistered?.by?.name,
            },
        },
        modifiedBy: {
            id: apiData.lastUpdated?.by?.id,
            text: {
                userName: apiData.lastUpdated?.by?.name,
            },
        },
        modified: apiData.lastUpdated?.atUtc,
        path:
            reversedPath.map(({ id, name }) => ({
                element: id,
                text: name,
            })) ?? [],
    }

    return serviceResponse
}
