import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch'
import { ServiceError } from '..'
import { getApiPaginationQueryParams } from '@helpers/routing/getApiPaginationQueryParams'
import { getClientPaginationFromApi } from '@helpers/routing/getClientPaginationFromApi'
import {
    ApiPaginatedResponse,
    ServicePaginatedResponse,
    ServiceResponse,
} from '@appTypes/service'
import { Pagination } from '@appTypes/pagination'
import { User } from '@appTypes/user'
import { Domain } from '@appTypes/domain'
import jwtHeader from '@helpers/util/jwt/jwtHeader'
import { api } from '@constants/routes'
import { getLastMonthDate } from '@helpers/util/date'

declare type Options = {
    user: User
    pagination?: Pagination
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export type ActiveUsers = {
    daily?: ActiveUsersResult
    weekly?: ActiveUsersResult
    monthly?: ActiveUsersResult
}

declare type ActiveUsersResult = {
    result: number
    label: string
}

export const getActiveUsers = async (
    { user, pagination }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<ActiveUsers>> => {
    const serviceResponse: ServiceResponse<ActiveUsers> = {
        data: { daily: undefined, weekly: undefined, monthly: undefined },
    }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}${api.ADMIN_ANALYTICS}`
    const apiResponse: any = await fetchJSON(
        apiUrl,
        setFetchOptions({
            method: requestMethods.GET,
            headers: {
                ...jwtHeader(user.accessToken),
            },
        }),
        defaultTimeOutMillis
    )
    const apiData: any = apiResponse.json
    const apiMeta: any = apiResponse.meta

    const { ok, status, statusText } = apiMeta
    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to get the number of active users',
            {
                serviceId: services.GET_ACTIVE_USERS,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    serviceResponse.data = Object.entries(apiData).reduce(
        (acc, [key, value]) => {
            let label = ''
            var dateLastMonth = getLastMonthDate()
            switch (key) {
                case 'daily':
                    label = 'Daily (last 24 hours)'
                    break
                case 'weekly':
                    label = 'Weekly (last 7 days)'
                    break
                case 'monthly':
                    label = `Monthly (from ${dateLastMonth})`
                    break
            }
            return {
                ...acc,
                [key]: {
                    result: value,
                    label,
                },
            }
        },
        serviceResponse.data
    )

    return serviceResponse
}
