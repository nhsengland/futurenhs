import {
    setFetchOpts as setFetchOptionsHelper,
    fetchJSON as fetchJSONHelper,
} from '@helpers/fetch'
import { services } from '@constants/services'
import { defaultTimeOutMillis, requestMethods } from '@constants/fetch'
import { ServiceError } from '..'
import { getApiPaginationQueryParams } from '@helpers/routing/getApiPaginationQueryParams'
import { getClientPaginationFromApi } from '@helpers/routing/getClientPaginationFromApi'
import { ServicePaginatedResponse } from '@appTypes/service'
import { FetchResponse } from '@appTypes/fetch'
import { ApiResponse } from '@appTypes/service'
import { DiscussionComment } from '@appTypes/discussion'
import { User } from '@appTypes/user'
import { Pagination } from '@appTypes/pagination'
import { mapToProfileImageObject } from '@helpers/util/data'
import jwtHeader from '@helpers/util/jwt/jwtHeader'

declare type Options = {
    groupId: string
    discussionId: string
    commentId: string
    user: User
    pagination?: Pagination
}

declare type Dependencies = {
    setFetchOptions: any
    fetchJSON: any
}

export const getGroupDiscussionCommentReplies = async (
    { groupId, discussionId, commentId, user, pagination }: Options,
    dependencies?: Dependencies
): Promise<ServicePaginatedResponse<Array<DiscussionComment>>> => {
    const serviceResponse: ServicePaginatedResponse<Array<DiscussionComment>> =
        {
            data: [],
        }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const { id } = user
    const paginationQueryParams: string = getApiPaginationQueryParams({
        pagination,
        defaults: {
            pageNumber: 1,
            pageSize: 30,
        },
    })

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/groups/${groupId}/discussions/${discussionId}/comments/${commentId}/replies?${paginationQueryParams}`
    const authHeader = jwtHeader(user.accessToken)
    const apiHeaders = setFetchOptions({
        method: requestMethods.GET,
        headers: authHeader,
    })
    const apiResponse: FetchResponse = await fetchJSON(
        apiUrl,
        apiHeaders,
        defaultTimeOutMillis
    )

    const apiData: ApiResponse<any> = apiResponse.json
    const apiMeta: any = apiResponse.meta

    const { ok, status, statusText } = apiMeta

    if (!ok) {
        throw new ServiceError(
            'An unexpected error occurred when attempting to get the group discussion comment replies',
            {
                serviceId: services.GET_GROUP_DISCUSSION_COMMENTS_WITH_REPLIES,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }

    apiData.data?.forEach((datum) => {
        serviceResponse.data.push({
            commentId: datum.id,
            originCommentId: datum.inReplyTo,
            text: {
                body: datum.content,
            },
            createdBy: {
                id: datum.firstRegistered?.by?.id ?? '',
                text: {
                    userName: datum.firstRegistered?.by?.name ?? '',
                },
                image: mapToProfileImageObject(datum.firstRegistered?.by?.image, 'Profile image') 
            },
            created: datum.firstRegistered?.atUtc ?? '',
            likeCount: datum.likesCount ?? 0,
            isLiked: datum.currentUser?.liked,
        })
    })

    serviceResponse.pagination = getClientPaginationFromApi({
        apiPaginatedResponse: apiData,
    })

    return serviceResponse
}
