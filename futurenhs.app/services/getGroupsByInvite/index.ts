import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch'
import { ServiceError } from '..'
import { FetchResponse } from '@appTypes/fetch'
import { ApiResponse, ServiceResponse } from '@appTypes/service'
import { Group, GroupInvitedBy } from '@appTypes/group'
import { mapGroupData } from '@helpers/formatters/mapGroupData'
import { api } from '@constants/routes'

declare type Response = {
    invitedBy: GroupInvitedBy | null
    group: Group | null
}

declare type Options = {
    id: string
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const getGroupsByInvite = async (
    { id }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<Response>> => {
    const serviceResponse: ServiceResponse<Response> = {
        data: null,
    }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const apiUrl: string = `${
        process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL
    }${api.SITE_INVITE_DETAILS.replace('%SITE_INVITE_ID%', id)}`
    const apiResponse: FetchResponse = await fetchJSON(
        apiUrl,
        setFetchOptions({ method: requestMethods.GET }),
        defaultTimeOutMillis
    )

    const apiData: ApiResponse<Response> = apiResponse.json
    const apiMeta: any = apiResponse.meta

    const { ok, status, statusText, headers } = apiMeta

    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to get the group invite',
            {
                serviceId: services.GET_INVITE_DETAILS,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }
    const group = mapGroupData(apiData.group)
    serviceResponse.headers = headers
    serviceResponse.data = {
        invitedBy: apiData.invitedBy,
        group,
    }

    return serviceResponse
}
