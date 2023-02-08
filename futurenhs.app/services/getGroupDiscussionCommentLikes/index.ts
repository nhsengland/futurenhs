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

export interface CommentLike {
    id: string
    createdByThisUser?: boolean
    createdAtUtc?: string
    firstRegistered?: {
        atUtc?: string
        by?: {
            id?: string
            name?: string
            slug?: string
            image?: ImageData
        }
    }
}

export const getGroupDiscussionCommentLikes = async (
    { groupId, discussionId, commentId, user, pagination }: Options,
    dependencies?: Dependencies
): Promise<ServiceResponse<Array<CommentLike>>> => {
    const serviceResponse: ServiceResponse<Array<CommentLike>> = {
        data: [],
    }

    const setFetchOptions =
        dependencies?.setFetchOptions ?? setFetchOptionsHelper
    const fetchJSON = dependencies?.fetchJSON ?? fetchJSONHelper

    const apiUrl: string = `${process.env.NEXT_PUBLIC_API_GATEWAY_BASE_URL}/v1/groups/${groupId}/discussions/${discussionId}/comments/${commentId}/likes`
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
            'An unexpected error occurred when attempting to get the list of users who have liked the comment',
            {
                serviceId: services.GET_GROUP_DISCUSSION_COMMENT_LIKES,
                status: status,
                statusText: statusText,
                body: apiData,
            }
        )
    }
    
    serviceResponse.data = apiData
    return serviceResponse
}

